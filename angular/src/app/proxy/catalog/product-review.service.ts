import type { CreateProductReviewDto, ProductReviewAggregateDto, ProductReviewDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductReviewService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getAggregate = (productId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductReviewAggregateDto>({
      method: 'GET',
      url: `/api/app/product-review/products/${productId}/aggregate`,
    },
    { apiName: this.apiName,...config });
  

  getList = (productId: string, input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductReviewDto>>({
      method: 'GET',
      url: `/api/app/product-review/products/${productId}/reviews`,
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  submit = (input: CreateProductReviewDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductReviewDto>({
      method: 'POST',
      url: '/api/app/product-review',
      body: input,
    },
    { apiName: this.apiName,...config });
}