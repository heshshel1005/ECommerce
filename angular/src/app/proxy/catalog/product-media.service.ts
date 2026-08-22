import type { ProductMediaDto, UpdateProductMediaDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { IActionResult } from '../microsoft/asp-net-core/mvc/models';
import type { ProductMediaUploadRequest } from '../models/catalog/models';

@Injectable({
  providedIn: 'root',
})
export class ProductMediaService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/product-media/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductMediaDto>({
      method: 'GET',
      url: `/api/app/product-media/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getFile = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IActionResult>({
      method: 'GET',
      url: `/api/app/product-media/${id}/file`,
    },
    { apiName: this.apiName,...config });
  

  getListByProductId = (productId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductMediaDto[]>({
      method: 'GET',
      url: `/api/app/product-media/by-product/${productId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateProductMediaDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductMediaDto>({
      method: 'PUT',
      url: `/api/app/product-media/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  upload = (request: ProductMediaUploadRequest, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ProductMediaDto>({
      method: 'POST',
      url: '/api/app/product-media/upload',
      body: request.file,
    },
    { apiName: this.apiName,...config });
}