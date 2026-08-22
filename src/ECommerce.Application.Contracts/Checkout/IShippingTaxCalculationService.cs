using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Checkout;

/// <summary>
/// Calculates shipping options and tax for a given cart/subtotal. Rules by region/weight; can integrate tax provider later.
/// </summary>
public interface IShippingTaxCalculationService : IApplicationService
{
    /// <summary>
    /// Returns available shipping options for the given subtotal and optional shipping country/region.
    /// </summary>
    Task<List<ShippingOptionDto>> GetShippingOptionsAsync(decimal subtotal, string? countryCode = null, string? regionCode = null);

    /// <summary>
    /// Calculates tax amount for the given subtotal and optional shipping address (for future region-based rules).
    /// </summary>
    Task<decimal> CalculateTaxAsync(decimal subtotal, string? countryCode = null, string? regionCode = null);
}
