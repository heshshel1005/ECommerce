import type { WishlistDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class WishlistService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addItem = (productVariantId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WishlistDto>({
      method: 'POST',
      url: '/api/app/wishlist/items',
      params: { productVariantId },
    },
    { apiName: this.apiName,...config });
  

  addToCart = (wishlistItemId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/wishlist/items/${wishlistItemId}/add-to-cart`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, WishlistDto>({
      method: 'GET',
      url: '/api/app/wishlist',
    },
    { apiName: this.apiName,...config });
  

  removeItem = (wishlistItemId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, WishlistDto>({
      method: 'DELETE',
      url: `/api/app/wishlist/items/${wishlistItemId}`,
    },
    { apiName: this.apiName,...config });
}