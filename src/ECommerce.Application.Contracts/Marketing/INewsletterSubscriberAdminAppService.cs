using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace ECommerce.Marketing;

/// <summary>
/// Admin: list newsletter subscribers (subscription list).
/// </summary>
public interface INewsletterSubscriberAdminAppService : IApplicationService
{
    Task<PagedResultDto<NewsletterSubscriberDto>> GetListAsync(NewsletterSubscriberListRequestDto input);
}

public class NewsletterSubscriberListRequestDto : PagedResultRequestDto
{
    public bool? IsActiveOnly { get; set; }
}
