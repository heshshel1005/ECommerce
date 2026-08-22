using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.Application.Dtos;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/newsletter-admin")]
[Area("app")]
public class NewsletterAdminController : ECommerceController
{
    private readonly INewsletterSubscriberAdminAppService _subscriberAppService;
    private readonly INewsletterCampaignAppService _campaignAppService;

    public NewsletterAdminController(
        INewsletterSubscriberAdminAppService subscriberAppService,
        INewsletterCampaignAppService campaignAppService)
    {
        _subscriberAppService = subscriberAppService;
        _campaignAppService = campaignAppService;
    }

    [HttpGet("subscribers")]
    public async Task<PagedResultDto<NewsletterSubscriberDto>> GetSubscribersAsync([FromQuery] NewsletterSubscriberListRequestDto input)
    {
        return await _subscriberAppService.GetListAsync(input);
    }

    [HttpPost("campaign/send")]
    public async Task SendCampaignAsync([FromBody] SendNewsletterCampaignDto input)
    {
        await _campaignAppService.SendCampaignAsync(input);
    }
}
