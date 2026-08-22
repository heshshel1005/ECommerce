using System;
using System.Linq;
using System.Threading.Tasks;
using ECommerce.Cart;
using ECommerce.Marketing;
using ECommerce.Orders;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Checkout;

/// <summary>
/// Checkout: summary (cart + shipping/tax), submit order from cart with contact and addresses.
/// API exposed via CheckoutController only (not auto-exposed by ABP).
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public class CheckoutAppService : ECommerceAppService, ICheckoutAppService
{
    private readonly ICartAppService _cartAppService;
    private readonly IShippingTaxCalculationService _shippingTax;
    private readonly IRepository<Cart.Cart, Guid> _cartRepository;
    private readonly IRepository<Cart.CartItem, Guid> _cartItemRepository;
    private readonly IRepository<Order, Guid> _orderRepository;
    private readonly IRepository<OrderLine, Guid> _orderLineRepository;
    private readonly IRepository<OrderStatusHistory, Guid> _orderStatusHistoryRepository;
    private readonly IRepository<Catalog.ProductVariant, Guid> _variantRepository;
    private readonly IRepository<Catalog.Product, Guid> _productRepository;
    private readonly Catalog.IInventoryValidationAppService _inventoryValidation;
    private readonly Catalog.IInventoryReservationService _inventoryReservation;
    private readonly IRepository<Coupon, Guid> _couponRepository;
    private readonly IRepository<CouponUsage, Guid> _couponUsageRepository;

    public CheckoutAppService(
        ICartAppService cartAppService,
        IShippingTaxCalculationService shippingTax,
        IRepository<Cart.Cart, Guid> cartRepository,
        IRepository<Cart.CartItem, Guid> cartItemRepository,
        IRepository<Order, Guid> orderRepository,
        IRepository<OrderLine, Guid> orderLineRepository,
        IRepository<OrderStatusHistory, Guid> orderStatusHistoryRepository,
        IRepository<Catalog.ProductVariant, Guid> variantRepository,
        IRepository<Catalog.Product, Guid> productRepository,
        Catalog.IInventoryValidationAppService inventoryValidation,
        Catalog.IInventoryReservationService inventoryReservation,
        IRepository<Coupon, Guid> couponRepository,
        IRepository<CouponUsage, Guid> couponUsageRepository)
    {
        _cartAppService = cartAppService;
        _shippingTax = shippingTax;
        _cartRepository = cartRepository;
        _cartItemRepository = cartItemRepository;
        _orderRepository = orderRepository;
        _orderLineRepository = orderLineRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _variantRepository = variantRepository;
        _productRepository = productRepository;
        _inventoryValidation = inventoryValidation;
        _inventoryReservation = inventoryReservation;
        _couponRepository = couponRepository;
        _couponUsageRepository = couponUsageRepository;
    }

    [AllowAnonymous]
    public async Task<CheckoutSummaryDto> GetSummaryAsync(Guid? guestCartId = null, string? couponCode = null)
    {
        var cart = await _cartAppService.GetCartAsync(guestCartId);
        var subtotal = cart.Items == null || cart.Items.Count == 0
            ? 0
            : cart.Items.Sum(i => (i.UnitPrice ?? 0) * i.Quantity);
        var (appliedCoupon, discountAmount) = await ResolveCouponDiscountAsync(couponCode, subtotal);
        var shippingOptions = await _shippingTax.GetShippingOptionsAsync(subtotal);
        var defaultCode = shippingOptions.FirstOrDefault()?.Code;
        var taxAmount = await _shippingTax.CalculateTaxAsync(subtotal);

        return new CheckoutSummaryDto
        {
            Cart = cart,
            SubTotal = subtotal,
            DiscountAmount = discountAmount,
            AppliedCouponCode = appliedCoupon?.Code,
            ShippingOptions = shippingOptions,
            TaxAmount = taxAmount,
            DefaultShippingMethodCode = defaultCode,
        };
    }

    [AllowAnonymous]
    public async Task<SubmitCheckoutResultDto> SubmitOrderAsync(SubmitCheckoutDto input, Guid? guestCartId = null)
    {
        var cartDto = await _cartAppService.GetCartAsync(guestCartId);
        if (cartDto.Items == null || cartDto.Items.Count == 0)
            throw new Volo.Abp.BusinessException("ECommerce:CartEmpty").WithData("Message", "Cart is empty.");

        var cart = await _cartRepository.GetAsync(cartDto.Id);
        var items = await _cartItemRepository.GetListAsync(x => x.CartId == cart.Id);

        foreach (var item in items)
            await _inventoryValidation.ValidateVariantAvailabilityAsync(item.ProductVariantId, item.Quantity);

        var subtotal = cartDto.Items.Sum(i => (i.UnitPrice ?? 0) * i.Quantity);
        var shippingOptions = await _shippingTax.GetShippingOptionsAsync(subtotal);
        var selected = shippingOptions.FirstOrDefault(x => x.Code == input.ShippingMethodCode)
            ?? shippingOptions.FirstOrDefault()
            ?? throw new Volo.Abp.BusinessException("ECommerce:InvalidShippingMethod").WithData("Code", input.ShippingMethodCode);
        var shippingAmount = selected.Amount;
        var taxAmount = await _shippingTax.CalculateTaxAsync(subtotal);
        var (appliedCoupon, discountAmount) = await ResolveCouponDiscountAsync(input.CouponCode, subtotal);
        var total = Math.Max(0, subtotal - discountAmount) + shippingAmount + taxAmount;

        var shipping = input.ShippingAddress ?? throw new Volo.Abp.BusinessException("ECommerce:ShippingAddressRequired");
        var order = new Order(
            GuidGenerator.Create(),
            cart.UserId,
            input.ContactEmail,
            input.ContactPhone,
            input.ContactName,
            shipping.Street,
            shipping.Street2,
            shipping.City,
            shipping.Region,
            shipping.PostalCode,
            shipping.Country,
            shipping.DeliveryInstructions,
            input.BillingSameAsShipping,
            selected.Code,
            selected.Name,
            shippingAmount,
            taxAmount,
            subtotal,
            total,
            appliedCoupon?.Id,
            discountAmount,
            input.BillingSameAsShipping ? null : input.BillingAddress?.Street,
            input.BillingSameAsShipping ? null : input.BillingAddress?.Street2,
            input.BillingSameAsShipping ? null : input.BillingAddress?.City,
            input.BillingSameAsShipping ? null : input.BillingAddress?.Region,
            input.BillingSameAsShipping ? null : input.BillingAddress?.PostalCode,
            input.BillingSameAsShipping ? null : input.BillingAddress?.Country);

        var variantIds = items.Select(x => x.ProductVariantId).Distinct().ToList();
        var variants = await _variantRepository.GetListAsync(v => variantIds.Contains(v.Id));
        var productIds = variants.Select(v => v.ProductId).Distinct().ToList();
        var products = await _productRepository.GetListAsync(p => productIds.Contains(p.Id));
        var variantMap = variants.ToDictionary(v => v.Id);
        var productMap = products.ToDictionary(p => p.Id);

        foreach (var item in items)
        {
            variantMap.TryGetValue(item.ProductVariantId, out var variant);
            var product = variant != null && productMap.TryGetValue(variant.ProductId, out var p) ? p : null;
            var productName = product?.Name ?? "";
            var sku = variant?.Sku ?? "";
            var unitPrice = variant?.Price ?? 0;
            var line = new OrderLine(
                GuidGenerator.Create(),
                order.Id,
                item.ProductVariantId,
                variant?.ProductId ?? Guid.Empty,
                productName,
                sku,
                unitPrice,
                item.Quantity);
            order.AddLine(line);
        }

        await _orderRepository.InsertAsync(order);

        if (appliedCoupon != null)
        {
            var usage = new CouponUsage(GuidGenerator.Create(), appliedCoupon.Id, cart.UserId, order.Id);
            await _couponUsageRepository.InsertAsync(usage);
        }

        var initialHistory = new OrderStatusHistory(GuidGenerator.Create(), order.Id, OrderStatus.Pending);
        await _orderStatusHistoryRepository.InsertAsync(initialHistory);

        var cartItemPairs = items.Select(i => (i.ProductVariantId, i.Quantity)).ToList();
        await _inventoryReservation.ReleaseForCartItemsAsync(cartItemPairs);

        await _cartItemRepository.DeleteManyAsync(items);
        await _cartRepository.DeleteAsync(cart);

        return new SubmitCheckoutResultDto
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
        };
    }

    /// <summary>
    /// Resolves coupon by code and returns (coupon, discountAmount) if valid; otherwise (null, 0).
    /// Validates: exists, active, validity dates, min order, total usage limit, per-user usage limit.
    /// </summary>
    private async Task<(Coupon? coupon, decimal discountAmount)> ResolveCouponDiscountAsync(string? code, decimal subtotal)
    {
        if (string.IsNullOrWhiteSpace(code)) return (null, 0);
        var coupon = await _couponRepository.FirstOrDefaultAsync(c =>
            c.Code == code.Trim().ToUpperInvariant());
        if (coupon == null) return (null, 0);
        if (!coupon.IsValidAt(DateTime.UtcNow)) return (null, 0);
        if (subtotal < coupon.MinOrderAmount) return (null, 0);

        var totalUsages = await _couponUsageRepository.CountAsync(x => x.CouponId == coupon.Id);
        if (coupon.TotalUsageLimit.HasValue && totalUsages >= coupon.TotalUsageLimit.Value)
            return (null, 0);

        var userId = CurrentUser.Id;
        if (coupon.PerUserUsageLimit.HasValue && userId.HasValue)
        {
            var userUsages = await _couponUsageRepository.CountAsync(x =>
                x.CouponId == coupon.Id && x.UserId == userId);
            if (userUsages >= coupon.PerUserUsageLimit.Value)
                return (null, 0);
        }

        var discount = coupon.CalculateDiscount(subtotal);
        return (coupon, discount);
    }
}
