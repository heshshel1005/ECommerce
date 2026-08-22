import type { AddCartItemDto, CartDto, MergeGuestCartRequest, UpdateCartItemDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CartService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  addItem = (input: AddCartItemDto, guestCartId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CartDto>({
      method: 'POST',
      url: '/api/app/cart/items',
      params: { guestCartId },
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getCart = (guestCartId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CartDto>({
      method: 'GET',
      url: '/api/app/cart',
      params: { guestCartId },
    },
    { apiName: this.apiName,...config });
  

  mergeGuestCart = (request: MergeGuestCartRequest, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CartDto>({
      method: 'POST',
      url: '/api/app/cart/merge-guest',
      body: request,
    },
    { apiName: this.apiName,...config });
  

  removeItem = (cartItemId: string, guestCartId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CartDto>({
      method: 'DELETE',
      url: `/api/app/cart/items/${cartItemId}`,
      params: { guestCartId },
    },
    { apiName: this.apiName,...config });
  

  updateItem = (cartItemId: string, input: UpdateCartItemDto, guestCartId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CartDto>({
      method: 'PUT',
      url: `/api/app/cart/items/${cartItemId}`,
      params: { guestCartId },
      body: input,
    },
    { apiName: this.apiName,...config });
}