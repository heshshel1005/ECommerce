import type { AddGiftRegistryItemDto, ClaimRegistryItemDto, CreateGiftRegistryDto, GiftRegistryDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class GiftRegistryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addItem = (giftRegistryId: string, input: AddGiftRegistryItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GiftRegistryDto>({
      method: 'POST',
      url: `/api/app/gift-registry/${giftRegistryId}/items`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  claim = (input: ClaimRegistryItemDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/gift-registry/claim',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  create = (input: CreateGiftRegistryDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GiftRegistryDto>({
      method: 'POST',
      url: '/api/app/gift-registry',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getBySlug = (slug: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GiftRegistryDto>({
      method: 'GET',
      url: `/api/app/gift-registry/by-slug/${slug}`,
    },
    { apiName: this.apiName,...config });
  

  getMyRegistries = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, GiftRegistryDto[]>({
      method: 'GET',
      url: '/api/app/gift-registry/my',
    },
    { apiName: this.apiName,...config });
  

  removeItem = (giftRegistryId: string, giftRegistryItemId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, GiftRegistryDto>({
      method: 'DELETE',
      url: `/api/app/gift-registry/${giftRegistryId}/items/${giftRegistryItemId}`,
    },
    { apiName: this.apiName,...config });
}