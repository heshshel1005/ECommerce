using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Emailing;
using Volo.Abp.Identity;

namespace ECommerce.Email
{
    public class EmailAppService : ApplicationService, IEmailAppService
    {
        private readonly IEmailSender _emailSender;
        private readonly IIdentityUserRepository _identityUserRepository;
        private readonly IdentityUserManager _userManager;

        public EmailAppService(IEmailSender emailSender, IIdentityUserRepository identityUserRepository, IdentityUserManager userManager)
        {
            _emailSender = emailSender;
            _identityUserRepository = identityUserRepository;
            _userManager = userManager;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            await _emailSender.SendAsync(to, subject, body);
        }

        public async Task SendEmailToUserAsync(Guid userId, string subject, string body)
        {
            var user = await _identityUserRepository.FindAsync(userId);
            if (user == null)
            {
                throw new UserFriendlyException($"User with id '{userId}' not found.");
            }
            await _emailSender.SendAsync(user.Email, subject, body);
        }

        public async Task SendEmailToUserNameAsync(string userName, string subject, string body)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                throw new UserFriendlyException($"User with username '{userName}' not found.");
            }
            await _emailSender.SendAsync(user.Email, subject, body);
        }
    }
}
