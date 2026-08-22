using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Catalog;

/// <summary>
/// Validates product variant stock. Used at add-to-cart and order submit.
/// </summary>
public class InventoryValidationAppService : ECommerceAppService, IInventoryValidationAppService
{
    private readonly IRepository<Inventory, Guid> _inventoryRepository;

    public InventoryValidationAppService(IRepository<Inventory, Guid> inventoryRepository)
    {
        _inventoryRepository = inventoryRepository;
    }

    public async Task ValidateVariantAvailabilityAsync(Guid productVariantId, int quantity)
    {
        if (quantity <= 0)
            return;

        var inv = await _inventoryRepository.FirstOrDefaultAsync(i => i.ProductVariantId == productVariantId);
        var available = inv?.AvailableQuantity ?? 0;
        if (available < quantity)
            throw new Volo.Abp.BusinessException("ECommerce:InsufficientStock")
                .WithData("ProductVariantId", productVariantId)
                .WithData("Requested", quantity)
                .WithData("Available", available);
    }
}
