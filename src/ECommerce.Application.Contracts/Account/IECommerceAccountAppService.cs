using System.Threading.Tasks;
using Volo.Abp.Identity;

namespace ECommerce.Account;

/// <summary>
/// Extended account app service: full customer subscription (account + contact + addresses), email confirmation.
/// </summary>
public interface IECommerceAccountAppService
{
    Task<IdentityUserDto> SubscribeAsync(CustomerRegisterDto input);

    Task ConfirmEmailAsync(ConfirmEmailInputDto input);
}
