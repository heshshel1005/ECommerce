import type { CreateProductDto, ProductAttributeDto, ProductDto, ProductListDto, ProductListRequestDto, UpdateProductDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ProductService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'POST',
      url: '/api/app/product',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/product/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'GET',
      url: `/api/app/product/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getAttributes = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductAttributeDto[]>({
      method: 'GET',
      url: '/api/app/product/attributes',
    },
    { apiName: this.apiName,...config });
  

  getList = (input: ProductListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<ProductListDto>>({
      method: 'GET',
      url: '/api/app/product/list',
      params: { filter: input.filter, categoryId: input.categoryId, isPublished: input.isPublished, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateProductDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductDto>({
      method: 'PUT',
      url: `/api/app/product/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}