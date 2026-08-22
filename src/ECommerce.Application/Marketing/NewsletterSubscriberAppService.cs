using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
public class NewsletterSubscriberAppService : ECommerceAppService, INewsletterSubscriberAppService
{
    private readonly IRepository<NewsletterSubscriber, Guid> _repository;

    public NewsletterSubscriberAppService(IRepository<NewsletterSubscriber, Guid> repository)
    {
        _repository = repository;
    }

    [Authorize]
    public async Task<NewsletterSubscriptionStatusDto> GetMyStatusAsync()
    {
        var email = CurrentUser.Email;
        if (string.IsNullOrEmpty(email))
            return new NewsletterSubscriptionStatusDto { IsSubscribed = false };

        var subscriber = await _repository.FirstOrDefaultAsync(s => s.Email == email.Trim().ToLowerInvariant());
        return new NewsletterSubscriptionStatusDto { IsSubscribed = subscriber?.IsActive ?? false };
    }

    [Authorize]
    public async Task SubscribeAsync(SubscribeNewsletterDto input)
    {
        var email = (CurrentUser.Email ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(email))
            throw new Volo.Abp.BusinessException("ECommerce:NewsletterEmailRequired");

        var existing = await _repository.FirstOrDefaultAsync(s => s.Email == email);
        if (existing != null)
        {
            if (existing.IsActive)
                return;
            existing.Resubscribe();
            await _repository.UpdateAsync(existing);
            return;
        }

        var subscriber = new NewsletterSubscriber(GuidGenerator.Create(), email, input.Name?.Trim());
        await _repository.InsertAsync(subscriber);
    }

    public async Task UnsubscribeAsync(string? email = null)
    {
        string normalized;
        if (CurrentUser.IsAuthenticated && string.IsNullOrWhiteSpace(email))
        {
            normalized = (CurrentUser.Email ?? string.Empty).Trim().ToLowerInvariant();
        }
        else
        {
            normalized = (email ?? string.Empty).Trim().ToLowerInvariant();
        }

        if (string.IsNullOrEmpty(normalized)) return;

        var subscriber = await _repository.FirstOrDefaultAsync(s => s.Email == normalized);
        if (subscriber == null) return;

        subscriber.Unsubscribe();
        await _repository.UpdateAsync(subscriber);
    }
}
