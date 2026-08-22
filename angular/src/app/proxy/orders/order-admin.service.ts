import type { CreateShipmentDto, OrderDto, OrderListDto, OrderListRequestDto, RefundOrderResultDto, ShipmentDto, UpdateOrderStatusDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class OrderAdminService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createShipment = (orderId: string, input: CreateShipmentDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ShipmentDto>({
      method: 'POST',
      url: `/api/app/order-admin/${orderId}/shipments`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>({
      method: 'GET',
      url: `/api/app/order-admin/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: OrderListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<OrderListDto>>({
      method: 'GET',
      url: '/api/app/order-admin',
      params: { status: input.status, dateFrom: input.dateFrom, dateTo: input.dateTo, search: input.search, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  getShipments = (orderId: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ShipmentDto[]>({
      method: 'GET',
      url: `/api/app/order-admin/${orderId}/shipments`,
    },
    { apiName: this.apiName,...config });
  

  refundOrder = (orderId: string, amount?: number, reason?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RefundOrderResultDto>({
      method: 'POST',
      url: `/api/app/order-admin/${orderId}/refund`,
      params: { amount, reason },
    },
    { apiName: this.apiName,...config });
  

  updateStatus = (id: string, input: UpdateOrderStatusDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrderDto>({
      method: 'PUT',
      url: `/api/app/order-admin/${id}/status`,
      body: input,
    },
    { apiName: this.apiName,...config });
}