using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

namespace ECommerce.Catalog;

[RemoteService(IsEnabled = false)]

[Authorize(ECommerce.Permissions.ECommercePermissions.Catalog.Default)]
public class ProductReviewAdminAppService : ECommerceAppService, IProductReviewAdminAppService
{
    private readonly IRepository<ProductReview, Guid> _reviewRepository;
    private readonly IRepository<Customers.CustomerProfile, Guid> _customerProfileRepository;
    private readonly IIdentityUserRepository _userRepository;

    public ProductReviewAdminAppService(
        IRepository<ProductReview, Guid> reviewRepository,
        IRepository<Customers.CustomerProfile, Guid> customerProfileRepository,
        IIdentityUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _customerProfileRepository = customerProfileRepository;
        _userRepository = userRepository;
    }

    public async Task<PagedResultDto<ProductReviewDto>> GetListAsync(ProductReviewListRequestDto input)
    {
        var query = await _reviewRepository.GetQueryableAsync();
        if (input.ProductId.HasValue)
            query = query.Where(x => x.ProductId == input.ProductId.Value);
        if (input.Status.HasValue)
            query = query.Where(x => x.Status == input.Status.Value);
        var total = await AsyncExecuter.CountAsync(query);
        var sorting = input.Sorting ?? nameof(ProductReview.CreationTime) + " DESC";
        var sortDesc = sorting.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase);
        var sortKey = sorting.Replace(" DESC", "", StringComparison.OrdinalIgnoreCase).Trim();
        query = sortKey switch
        {
            nameof(ProductReview.Rating) => sortDesc ? query.OrderByDescending(x => x.Rating) : query.OrderBy(x => x.Rating),
            nameof(ProductReview.CreationTime) => sortDesc ? query.OrderByDescending(x => x.CreationTime) : query.OrderBy(x => x.CreationTime),
            nameof(ProductReview.Status) => sortDesc ? query.OrderByDescending(x => x.Status) : query.OrderBy(x => x.Status),
            _ => query.OrderByDescending(x => x.CreationTime)
        };
        var skip = input.SkipCount;
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 20;
        var reviews = await AsyncExecuter.ToListAsync(query.Skip(skip).Take(take));
        var dtos = new System.Collections.Generic.List<ProductReviewDto>();
        foreach (var r in reviews)
            dtos.Add(await MapToDtoAsync(r));
        return new PagedResultDto<ProductReviewDto>(total, dtos);
    }

    public async Task ApproveAsync(Guid id)
    {
        var review = await _reviewRepository.GetAsync(id);
        review.Status = ProductReviewStatus.Approved;
        await _reviewRepository.UpdateAsync(review);
    }

    public async Task RejectAsync(Guid id)
    {
        var review = await _reviewRepository.GetAsync(id);
        review.Status = ProductReviewStatus.Rejected;
        await _reviewRepository.UpdateAsync(review);
    }

    public async Task DeleteAsync(Guid id)
    {
        await _reviewRepository.DeleteAsync(id);
    }

    private async Task<ProductReviewDto> MapToDtoAsync(ProductReview r)
    {
        var dto = new ProductReviewDto
        {
            Id = r.Id,
            ProductId = r.ProductId,
            UserId = r.UserId,
            Rating = r.Rating,
            ReviewText = r.ReviewText,
            Status = r.Status,
            CreationTime = r.CreationTime
        };
        dto.AuthorDisplayName = await GetDisplayNameAsync(r.UserId);
        return dto;
    }

    private async Task<string> GetDisplayNameAsync(Guid userId)
    {
        var profile = await _customerProfileRepository.FirstOrDefaultAsync(p => p.UserId == userId);
        if (!string.IsNullOrWhiteSpace(profile?.DisplayName))
            return profile.DisplayName;
        var user = await _userRepository.FindAsync(userId);
        if (user != null)
            return user.Name ?? user.UserName ?? user.Email ?? userId.ToString();
        return userId.ToString();
    }
}
