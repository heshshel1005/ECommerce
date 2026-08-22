import type { BrandModelDto, CreateBrandModelDto, UpdateBrandModelDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class BrandModelService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateBrandModelDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandModelDto>({
      method: 'POST',
      url: '/api/app/brand-model',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/brand-model/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandModelDto>({
      method: 'GET',
      url: `/api/app/brand-model/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (brandId?: string, isActive?: boolean, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandModelDto[]>({
      method: 'GET',
      url: '/api/app/brand-model',
      params: { brandId, isActive },
    },
    { apiName: this.apiName,...config });
  

  getListByBrandId = (brandId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandModelDto[]>({
      method: 'GET',
      url: `/api/app/brand-model/by-brand-id/${brandId}`,
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateBrandModelDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, BrandModelDto>({
      method: 'PUT',
      url: `/api/app/brand-model/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}