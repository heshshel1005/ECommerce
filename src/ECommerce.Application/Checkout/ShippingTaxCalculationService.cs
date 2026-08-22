using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.Checkout;

/// <summary>
/// Simple shipping and tax calculation. Fixed rules; can be extended with region/weight and tax provider.
/// </summary>
public class ShippingTaxCalculationService : ECommerceAppService, IShippingTaxCalculationService
{
    private const string StandardCode = "standard";
    private const string ExpressCode = "express";
    private const decimal StandardAmount = 5.00m;
    private const decimal ExpressAmount = 12.00m;
    private const decimal TaxRate = 0.10m; // 10% for simplicity

    public Task<List<ShippingOptionDto>> GetShippingOptionsAsync(decimal subtotal, string? countryCode = null, string? regionCode = null)
    {
        var list = new List<ShippingOptionDto>
        {
            new() { Code = StandardCode, Name = L["ECommerce:ShippingStandard"], Amount = StandardAmount },
            new() { Code = ExpressCode, Name = L["ECommerce:ShippingExpress"], Amount = ExpressAmount },
        };
        return Task.FromResult(list);
    }

    public Task<decimal> CalculateTaxAsync(decimal subtotal, string? countryCode = null, string? regionCode = null)
    {
        var tax = subtotal * TaxRate;
        return Task.FromResult(tax);
    }
}
