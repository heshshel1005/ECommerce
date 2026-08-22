using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class NewsletterCampaignAppService : ECommerceAppService, INewsletterCampaignAppService
{
    private readonly IRepository<NewsletterSubscriber, Guid> _subscriberRepository;
    private readonly IEmailSender _emailSender;
    private readonly ILogger<NewsletterCampaignAppService> _logger;

    public NewsletterCampaignAppService(
        IRepository<NewsletterSubscriber, Guid> subscriberRepository,
        IEmailSender emailSender,
        ILogger<NewsletterCampaignAppService> logger)
    {
        _subscriberRepository = subscriberRepository;
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task SendCampaignAsync(SendNewsletterCampaignDto input)
    {
        var subscribers = await _subscriberRepository.GetListAsync(s => s.IsActive && s.UnsubscribedAt == null);
        foreach (var s in subscribers)
        {
            try
            {
                await _emailSender.SendAsync(s.Email, input.Subject, input.Body, input.IsBodyHtml);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to send newsletter campaign email to {Email}. Continuing with other subscribers.", s.Email);
            }
        }
    }
}
