using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Newsletter subscription for logged-in users (subscribe/unsubscribe from profile). Unsubscribe(email) remains for email-link flow.
/// </summary>
public interface INewsletterSubscriberAppService : IApplicationService
{
    /// <summary>Requires auth. Returns whether the current user's email is subscribed.</summary>
    Task<NewsletterSubscriptionStatusDto> GetMyStatusAsync();

    /// <summary>Requires auth. Subscribes the current user's email; optional name from input.</summary>
    Task SubscribeAsync(SubscribeNewsletterDto input);

    /// <summary>When email is null/empty and user is authenticated, unsubscribes current user. Otherwise requires email (e.g. for link in campaign).</summary>
    Task UnsubscribeAsync(string? email = null);
}
