import type { InventoryDto, InventoryListRequestDto, UpdateInventoryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InventoryAdminService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  ensureForVariant = (productVariantId: string, quantity?: number, lowStockThreshold?: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryDto>({
      method: 'POST',
      url: '/api/app/inventory-admin/ensure-for-variant',
      params: { productVariantId, quantity, lowStockThreshold },
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryDto>({
      method: 'GET',
      url: `/api/app/inventory-admin/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getByVariantId = (productVariantId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryDto>({
      method: 'GET',
      url: `/api/app/inventory-admin/by-variant/${productVariantId}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: InventoryListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<InventoryDto>>({
      method: 'GET',
      url: '/api/app/inventory-admin',
      params: { productVariantId: input.productVariantId, lowStockOnly: input.lowStockOnly, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateInventoryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, InventoryDto>({
      method: 'PUT',
      url: `/api/app/inventory-admin/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}