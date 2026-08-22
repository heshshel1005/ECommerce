using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ECommerce.OrganizationSignup;

public interface IOrganizationSignupHostAppService : IApplicationService
{
    Task<PagedResultDto<OrganizationSignupRequestDto>> GetListAsync(OrganizationSignupRequestListRequestDto input);

    Task<OrganizationSignupRequestDto> GetAsync(Guid id);

    Task ApproveAsync(Guid id);

    Task RejectAsync(Guid id, RejectOrganizationSignupDto input);

    Task<RepairTenantAdminPermissionsResultDto> RepairTenantAdminPermissionsAsync();
}
