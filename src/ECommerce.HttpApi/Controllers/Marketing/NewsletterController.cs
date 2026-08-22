using System.Threading.Tasks;
using ECommerce.Marketing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Marketing;

[Route("api/app/newsletter")]
[Area("app")]
public class NewsletterController : ECommerceController
{
    private readonly INewsletterSubscriberAppService _appService;

    public NewsletterController(INewsletterSubscriberAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("my-status")]
    [Authorize]
    public async Task<NewsletterSubscriptionStatusDto> GetMyStatusAsync()
    {
        return await _appService.GetMyStatusAsync();
    }

    [HttpPost("subscribe")]
    [Authorize]
    public async Task SubscribeAsync([FromBody] SubscribeNewsletterDto input)
    {
        await _appService.SubscribeAsync(input);
    }

    [HttpPost("unsubscribe")]
    public async Task UnsubscribeAsync([FromQuery] string? email = null)
    {
        await _appService.UnsubscribeAsync(email);
    }
}
