import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CartReservationExpiryService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  releaseExpiredReservations = (olderThanMinutes: number = 30, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'POST',
      url: '/api/app/cart-admin/release-expired-reservations',
      params: { olderThanMinutes },
    },
    { apiName: this.apiName,...config });
}