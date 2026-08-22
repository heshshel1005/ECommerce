import type { BrandDto, CreateBrandDto, UpdateBrandDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BrandService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateBrandDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandDto>({
      method: 'POST',
      url: '/api/app/brand',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/brand/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandDto>({
      method: 'GET',
      url: `/api/app/brand/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (isActive?: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandDto[]>({
      method: 'GET',
      url: '/api/app/brand',
      params: { isActive },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateBrandDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandDto>({
      method: 'PUT',
      url: `/api/app/brand/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}