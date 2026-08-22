import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface ProductReviewDto {
  id: string;
  productId: string;
  userId: string;
  authorDisplayName: string;
  rating: number;
  reviewText?: string | null;
  status: number;
  creationTime: string;
}

export interface ProductReviewAggregateDto {
  averageRating: number;
  totalCount: number;
}

export interface CreateProductReviewDto {
  productId: string;
  rating: number;
  reviewText?: string | null;
}

export interface ProductReviewListRequestDto {
  productId?: string | null;
  status?: number | null;
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

@Injectable({ providedIn: 'root' })
export class ProductReviewService {
  private readonly rest = inject(RestService);

  getAggregate(productId: string): Observable<ProductReviewAggregateDto> {
    return this.rest.request<void, ProductReviewAggregateDto>({
      method: 'GET',
      url: `/api/app/product-review/products/${productId}/aggregate`,
    });
  }

  getList(
    productId: string,
    params: { skipCount?: number; maxResultCount?: number; sorting?: string } = {}
  ): Observable<PagedResultDto<ProductReviewDto>> {
    const requestParams: Record<string, string | number> = {};
    if (params.skipCount != null) requestParams.SkipCount = params.skipCount;
    if (params.maxResultCount != null) requestParams.MaxResultCount = params.maxResultCount;
    if (params.sorting != null) requestParams.Sorting = params.sorting;
    return this.rest.request<void, PagedResultDto<ProductReviewDto>>({
      method: 'GET',
      url: `/api/app/product-review/products/${productId}/reviews`,
      params: requestParams,
    });
  }

  submit(input: CreateProductReviewDto): Observable<ProductReviewDto> {
    return this.rest.request<CreateProductReviewDto, ProductReviewDto>({
      method: 'POST',
      url: '/api/app/product-review',
      body: input,
    });
  }
}
