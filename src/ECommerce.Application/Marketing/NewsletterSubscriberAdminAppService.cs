using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace ECommerce.Marketing;

[Volo.Abp.RemoteService(IsEnabled = false)]
[Authorize("ECommerce.Administration")]
public class NewsletterSubscriberAdminAppService : ECommerceAppService, INewsletterSubscriberAdminAppService
{
    private readonly IRepository<NewsletterSubscriber, Guid> _repository;

    public NewsletterSubscriberAdminAppService(IRepository<NewsletterSubscriber, Guid> repository)
    {
        _repository = repository;
    }

    public async Task<Volo.Abp.Application.Dtos.PagedResultDto<NewsletterSubscriberDto>> GetListAsync(NewsletterSubscriberListRequestDto input)
    {
        var query = await _repository.GetQueryableAsync();
        if (input.IsActiveOnly == true)
            query = query.Where(s => s.IsActive && s.UnsubscribedAt == null);

        var total = await AsyncExecuter.CountAsync(query);
        var skip = input.SkipCount;
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 10;
        var list = await AsyncExecuter.ToListAsync(query.OrderBy(s => s.Email).Skip(skip).Take(take));
        return new Volo.Abp.Application.Dtos.PagedResultDto<NewsletterSubscriberDto>(
            total,
            list.Select(s => new NewsletterSubscriberDto
            {
                Id = s.Id,
                Email = s.Email,
                Name = s.Name,
                IsActive = s.IsActive,
                CreationTime = s.CreationTime,
                UnsubscribedAt = s.UnsubscribedAt,
            }).ToList());
    }
}
