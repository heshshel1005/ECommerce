import type { CatalogFilterOptionsDto, CategoryTreeDto, ProductDto, PublicProductListDto, PublicProductListRequestDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PublicCatalogService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getCategoryTree = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryTreeDto[]>({
      method: 'GET',
      url: '/api/app/public-catalog/categories/tree',
    },
    { apiName: this.apiName,...config });
  

  getCompare = (productIds?: string[], ids?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto[]>({
      method: 'GET',
      url: '/api/app/public-catalog/compare',
      params: { productIds, ids },
    },
    { apiName: this.apiName,...config });
  

  getFilterOptions = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CatalogFilterOptionsDto>({
      method: 'GET',
      url: '/api/app/public-catalog/filter-options',
    },
    { apiName: this.apiName,...config });
  

  getProductDetail = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/public-catalog/products/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getProductList = (input: PublicProductListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<PublicProductListDto>>({
      method: 'GET',
      url: '/api/app/public-catalog/products',
      params: { search: input.search, categoryId: input.categoryId, priceMin: input.priceMin, priceMax: input.priceMax, size: input.size, color: input.color, brandId: input.brandId, modelId: input.modelId, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}