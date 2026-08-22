using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ECommerce.Catalog;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc;

namespace ECommerce.Controllers.Catalog;

[Route("api/app/product-type-attribute-rule")]
[Area("app")]
public class ProductTypeAttributeRuleController : ECommerceController
{
    private readonly IProductTypeAttributeRuleAppService _appService;

    public ProductTypeAttributeRuleController(IProductTypeAttributeRuleAppService appService)
    {
        _appService = appService;
    }

    [HttpGet("list-by-product-type")]
    public async Task<List<ProductTypeAttributeRuleDto>> GetListByProductTypeAsync([FromQuery] Guid productTypeId)
    {
        return await _appService.GetListByProductTypeAsync(productTypeId);
    }

    [HttpPost("replace-for-product-type")]
    public async Task ReplaceForProductTypeAsync([FromQuery] Guid productTypeId, [FromBody] List<UpdateProductTypeAttributeRuleDto> input)
    {
        await _appService.ReplaceForProductTypeAsync(productTypeId, input);
    }
}
