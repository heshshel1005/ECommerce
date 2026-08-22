using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Admin: list subscribers, send campaign email to all active subscribers via ABP IEmailSender.
/// </summary>
public interface INewsletterCampaignAppService : IApplicationService
{
    Task SendCampaignAsync(SendNewsletterCampaignDto input);
}
