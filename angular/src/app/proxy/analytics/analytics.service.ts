import type { AnalyticsFilterDto, SalesByDayDto, SalesSummaryDto, TopProductDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { FileContentResult } from '../microsoft/asp-net-core/mvc/models';

@Injectable({
  providedIn: 'root',
})
export class AnalyticsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  exportSalesCsv = (input: AnalyticsFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, FileContentResult>({
      method: 'GET',
      url: '/api/app/analytics/export',
      params: { dateFrom: input.dateFrom, dateTo: input.dateTo },
    },
    { apiName: this.apiName,...config });
  

  getSalesByDay = (input: AnalyticsFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SalesByDayDto[]>({
      method: 'GET',
      url: '/api/app/analytics/by-day',
      params: { dateFrom: input.dateFrom, dateTo: input.dateTo },
    },
    { apiName: this.apiName,...config });
  

  getSalesSummary = (input: AnalyticsFilterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SalesSummaryDto>({
      method: 'GET',
      url: '/api/app/analytics/summary',
      params: { dateFrom: input.dateFrom, dateTo: input.dateTo },
    },
    { apiName: this.apiName,...config });
  

  getTopProducts = (input: AnalyticsFilterDto, maxCount: number = 10, config?: Partial<Rest.Config>) =>
    this.restService.request<any, TopProductDto[]>({
      method: 'GET',
      url: '/api/app/analytics/top-products',
      params: { dateFrom: input.dateFrom, dateTo: input.dateTo, maxCount },
    },
    { apiName: this.apiName,...config });
}