using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Volo.Abp.Account;
using Volo.Abp.Account.Emailing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Emailing;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using ECommerce.Customers;
using ECommerce.Identity;
using ECommerce.Localization;

namespace ECommerce.Account;

/// <summary>
/// Custom account app service: sends email confirmation after registration so that
/// activation is required before login (when RequireConfirmedEmail is enabled).
/// Full customer subscription (account + contact + addresses) and Customer role assignment.
/// </summary>
[Dependency(ReplaceServices = true)]
public class ECommerceAccountAppService : AccountAppService, IECommerceAccountAppService
{
    private readonly IEmailSender _emailSender;
    private readonly IConfiguration _configuration;
    private readonly IStringLocalizer<ECommerceResource> _eCommerceLocalizer;
    private readonly IRepository<CustomerProfile, Guid> _customerProfileRepository;
    private readonly IRepository<CustomerAddress, Guid> _customerAddressRepository;
    private readonly IGuidGenerator _guidGenerator;

    public ECommerceAccountAppService(
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository,
        IAccountEmailer accountEmailer,
        IdentitySecurityLogManager identitySecurityLogManager,
        IOptions<IdentityOptions> identityOptions,
        IEmailSender emailSender,
        IConfiguration configuration,
        IStringLocalizer<ECommerceResource> eCommerceLocalizer,
        IRepository<CustomerProfile, Guid> customerProfileRepository,
        IRepository<CustomerAddress, Guid> customerAddressRepository,
        IGuidGenerator guidGenerator)
        : base(userManager, roleRepository, accountEmailer, identitySecurityLogManager, identityOptions)
    {
        _emailSender = emailSender;
        _configuration = configuration;
        _eCommerceLocalizer = eCommerceLocalizer;
        _customerProfileRepository = customerProfileRepository;
        _customerAddressRepository = customerAddressRepository;
        _guidGenerator = guidGenerator;
    }

    /// <summary>
    /// Full customer subscription: account (email/password), contact (name, phone), default shipping and optional billing address.
    /// Creates user, assigns Customer role, persists profile and addresses, sends email confirmation (activation required).
    /// </summary>
    [AllowAnonymous]
    public virtual async Task<IdentityUserDto> SubscribeAsync(CustomerRegisterDto input)
    {
        var registerDto = new RegisterDto
        {
            UserName = input.UserName,
            EmailAddress = input.EmailAddress,
            Password = input.Password,
            AppName = input.AppName
        };

        var result = await RegisterAsync(registerDto);

        var user = await UserManager.FindByEmailAsync(input.EmailAddress);
        if (user == null)
            return result;

        // Allow immediate login without waiting for confirmation email (e.g. when email is not configured or not received).
        if (!user.EmailConfirmed)
        {
            user.SetEmailConfirmed(true);
            await UserManager.UpdateAsync(user);
        }

        // Re-apply password in case UpdateAsync or entity tracking affected the password hash (ensures login works).
        var resetToken = await UserManager.GeneratePasswordResetTokenAsync(user);
        (await UserManager.ResetPasswordAsync(user, resetToken, input.Password)).CheckErrors();

        await UserManager.AddToRoleAsync(user, ECommerceIdentityRolesDataSeedContributor.CustomerRoleName);

        var profile = new CustomerProfile(
            _guidGenerator.Create(),
            user.Id,
            input.DisplayName,
            input.PhoneNumber);
        await _customerProfileRepository.InsertAsync(profile);

        var shipping = input.ShippingAddress;
        var shippingAddress = new CustomerAddress(
            _guidGenerator.Create(),
            user.Id,
            shipping.Label,
            shipping.Street,
            shipping.City,
            shipping.Region,
            shipping.PostalCode,
            shipping.Country,
            isDefaultShipping: true,
            isDefaultBilling: input.BillingAddress == null);
        await _customerAddressRepository.InsertAsync(shippingAddress);

        if (input.BillingAddress != null)
        {
            var billing = input.BillingAddress;
            var billingAddress = new CustomerAddress(
                _guidGenerator.Create(),
                user.Id,
                billing.Label,
                billing.Street,
                billing.City,
                billing.Region,
                billing.PostalCode,
                billing.Country,
                isDefaultShipping: false,
                isDefaultBilling: true);
            await _customerAddressRepository.InsertAsync(billingAddress);
        }

        return result;
    }

    [AllowAnonymous]
    public virtual async Task ConfirmEmailAsync(ConfirmEmailInputDto input)
    {
        var user = await UserManager.GetByIdAsync(input.UserId);
        var confirmResult = await UserManager.ConfirmEmailAsync(user, input.Token);
        user = await UserManager.GetByIdAsync(input.UserId); // refresh so we see actual EmailConfirmed
        if (user.EmailConfirmed)
        {
            // Confirmation applied (this request or a previous one); treat as success so UI does not show "Invalid token".
            try { (await UserManager.UpdateSecurityStampAsync(user)).CheckErrors(); } catch { /* best effort */ }
            return;
        }
        if (!confirmResult.Succeeded)
            confirmResult.CheckErrors();
        (await UserManager.UpdateSecurityStampAsync(user)).CheckErrors();
    }

    public override async Task<IdentityUserDto> RegisterAsync(RegisterDto input)
    {
        var result = await base.RegisterAsync(input);

        // Send email confirmation link so the user can activate before logging in.
        var user = await UserManager.FindByEmailAsync(input.EmailAddress);
        if (user != null && !user.EmailConfirmed)
        {
            var token = await UserManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = GetEmailConfirmationCallbackUrl(user.Id, token);
            var subject = L["AbpAccount::EmailConfirmationSubject"];
            // Use a body template that explicitly includes the link placeholder {0} so the confirmation link is always present.
            var body = _eCommerceLocalizer["ConfirmEmailEmailBody", callbackUrl];
            await _emailSender.SendAsync(input.EmailAddress, subject, body);
        }

        return result;
    }

    private string GetEmailConfirmationCallbackUrl(Guid userId, string token)
    {
        // Link to Angular app so the user opens the SPA; the confirm-email page will call the API to confirm.
        var angularUrl = (_configuration["App:AngularUrl"] ?? "http://localhost:4200").TrimEnd('/');
        var callbackUrl = $"{angularUrl}/account/confirm-email?userId={Uri.EscapeDataString(userId.ToString())}&token={Uri.EscapeDataString(token)}";
        return callbackUrl;
    }
}
