using System.Threading.Tasks;
using ECommerce.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.Identity;

namespace ECommerce.Controllers.Account;

/// <summary>
/// Exposes customer subscription (SubscribeAsync) and email confirmation (ConfirmEmail) for the Angular app.
/// </summary>
[Route("api/account")]
public class AccountController : AbpControllerBase
{
    private readonly IECommerceAccountAppService _accountAppService;

    public AccountController(IECommerceAccountAppService accountAppService)
    {
        _accountAppService = accountAppService;
    }

    [HttpPost("subscribe")]
    [AllowAnonymous]
    public async Task<IdentityUserDto> SubscribeAsync([FromBody] CustomerRegisterDto input)
    {
        return await _accountAppService.SubscribeAsync(input);
    }

    [HttpPost("confirm-email")]
    [AllowAnonymous]
    public async Task ConfirmEmailAsync([FromBody] ConfirmEmailInputDto input)
    {
        await _accountAppService.ConfirmEmailAsync(input);
    }
}
