using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Cart;

/// <summary>
/// Releases reserved inventory for carts that have been inactive for a given period.
/// Call from admin or a background job to implement cart reserve timeout.
/// </summary>
[Volo.Abp.RemoteService(IsEnabled = false)]
public interface ICartReservationExpiryService : IApplicationService
{
    /// <summary>
    /// Finds carts not modified since longer than <paramref name="olderThan"/>, releases their reserved quantities.
    /// </summary>
    Task<int> ReleaseExpiredReservationsAsync(TimeSpan olderThan);
}
