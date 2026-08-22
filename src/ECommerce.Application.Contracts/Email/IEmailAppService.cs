using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace ECommerce.Email
{
    public interface IEmailAppService : IApplicationService
    {
        Task SendEmailAsync(string to, string subject, string body);
        Task SendEmailToUserAsync(Guid userId, string subject, string body);
        Task SendEmailToUserNameAsync(string userName, string subject, string body);
    }
}
