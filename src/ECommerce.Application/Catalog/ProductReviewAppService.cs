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
/// <summary>
/// Public API: aggregate and list for PDP; authenticated customers submit review.
/// </summary>
public class ProductReviewAppService : ECommerceAppService, IProductReviewAppService
{
    private readonly IRepository<ProductReview, Guid> _reviewRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<Customers.CustomerProfile, Guid> _customerProfileRepository;
    private readonly IIdentityUserRepository _userRepository;

    public ProductReviewAppService(
        IRepository<ProductReview, Guid> reviewRepository,
        IRepository<Product, Guid> productRepository,
        IRepository<Customers.CustomerProfile, Guid> customerProfileRepository,
        IIdentityUserRepository userRepository)
    {
        _reviewRepository = reviewRepository;
        _productRepository = productRepository;
        _customerProfileRepository = customerProfileRepository;
        _userRepository = userRepository;
    }

    [AllowAnonymous]
    public async Task<ProductReviewAggregateDto> GetAggregateAsync(Guid productId)
    {
        var query = await _reviewRepository.GetQueryableAsync();
        query = query.Where(x => x.ProductId == productId && x.Status == ProductReviewStatus.Approved);
        var list = await AsyncExecuter.ToListAsync(query);
        var count = list.Count;
        if (count == 0)
            return new ProductReviewAggregateDto { AverageRating = 0, TotalCount = 0 };
        var avg = list.Average(x => x.Rating);
        return new ProductReviewAggregateDto { AverageRating = Math.Round(avg, 2), TotalCount = count };
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<ProductReviewDto>> GetListAsync(Guid productId, PagedAndSortedResultRequestDto input)
    {
        var query = await _reviewRepository.GetQueryableAsync();
        query = query.Where(x => x.ProductId == productId && x.Status == ProductReviewStatus.Approved);
        var total = await AsyncExecuter.CountAsync(query);
        var sorting = input.Sorting ?? nameof(ProductReview.CreationTime) + " DESC";
        var sortDesc = sorting.EndsWith(" DESC", StringComparison.OrdinalIgnoreCase);
        var sortKey = sorting.Replace(" DESC", "", StringComparison.OrdinalIgnoreCase).Trim();
        query = sortKey switch
        {
            nameof(ProductReview.Rating) => sortDesc ? query.OrderByDescending(x => x.Rating) : query.OrderBy(x => x.Rating),
            nameof(ProductReview.CreationTime) => sortDesc ? query.OrderByDescending(x => x.CreationTime) : query.OrderBy(x => x.CreationTime),
            _ => query.OrderByDescending(x => x.CreationTime)
        };
        var skip = input.SkipCount;
        var take = input.MaxResultCount > 0 ? input.MaxResultCount : 10;
        var reviews = await AsyncExecuter.ToListAsync(query.Skip(skip).Take(take));
        var dtos = await MapToDtosAsync(reviews);
        return new PagedResultDto<ProductReviewDto>(total, dtos);
    }

    [Authorize]
    public async Task<ProductReviewDto> SubmitAsync(CreateProductReviewDto input)
    {
        var userId = CurrentUser.Id ?? throw new Volo.Abp.Authorization.AbpAuthorizationException("User must be logged in to submit a review.");
        var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == input.ProductId);
        if (product == null)
            throw new Volo.Abp.Domain.Entities.EntityNotFoundException(typeof(Product), input.ProductId);

        var existing = await _reviewRepository.FirstOrDefaultAsync(x => x.ProductId == input.ProductId && x.UserId == userId);
        if (existing != null)
        {
            existing.Rating = input.Rating;
            existing.ReviewText = input.ReviewText?.Trim();
            existing.Status = ProductReviewStatus.Pending;
            await _reviewRepository.UpdateAsync(existing);
            return await MapToDtoAsync(existing);
        }

        var review = new ProductReview(
            GuidGenerator.Create(),
            input.ProductId,
            userId,
            input.Rating,
            input.ReviewText?.Trim(),
            ProductReviewStatus.Pending);
        await _reviewRepository.InsertAsync(review);
        return await MapToDtoAsync(review);
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

    private async Task<System.Collections.Generic.List<ProductReviewDto>> MapToDtosAsync(System.Collections.Generic.List<ProductReview> reviews)
    {
        var list = new System.Collections.Generic.List<ProductReviewDto>();
        foreach (var r in reviews)
            list.Add(await MapToDtoAsync(r));
        return list;
    }
}
