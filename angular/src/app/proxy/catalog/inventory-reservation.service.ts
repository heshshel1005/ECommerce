import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InventoryReservationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  release = (productVariantId: string, quantity: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory-reservation/release/${productVariantId}`,
      params: { quantity },
    },
    { apiName: this.apiName,...config });
  

  releaseForCartItems = (items: Record<string, number>, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/inventory-reservation/release-for-cart-items',
      body: items,
    },
    { apiName: this.apiName,...config });
  

  reserve = (productVariantId: string, quantity: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory-reservation/reserve/${productVariantId}`,
      params: { quantity },
    },
    { apiName: this.apiName,...config });
}