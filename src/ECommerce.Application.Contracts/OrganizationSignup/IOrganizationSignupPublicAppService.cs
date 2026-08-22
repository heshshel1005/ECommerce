using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Services;

namespace ECommerce.OrganizationSignup;

public interface IOrganizationSignupPublicAppService : IApplicationService
{
    Task<OrganizationSignupLogoUploadDto> UploadLogoAsync(IFormFile file);

    Task<OrganizationSignupSubmitResultDto> SubmitAsync(OrganizationSignupSubmitDto input);
}
