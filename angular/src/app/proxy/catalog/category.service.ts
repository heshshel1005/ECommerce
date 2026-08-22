import type { CategoryDto, CategoryTreeDto, CreateCategoryDto, UpdateCategoryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'POST',
      url: '/api/app/category',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'GET',
      url: `/api/app/category/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto[]>({
      method: 'GET',
      url: '/api/app/category/list',
    },
    { apiName: this.apiName,...config });
  

  getTree = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryTreeDto[]>({
      method: 'GET',
      url: '/api/app/category/tree',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateCategoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CategoryDto>({
      method: 'PUT',
      url: `/api/app/category/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}