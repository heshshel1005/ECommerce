import type { OrderDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class OrderService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>({
      method: 'GET',
      url: `/api/app/order/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getMyOrders = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto[]>({
      method: 'GET',
      url: '/api/app/order/my-orders',
    },
    { apiName: this.apiName,...config });
}