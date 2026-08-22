import type { CouponDto, CreateCouponDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedAndSortedResultRequestDto, PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CouponAdminService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateCouponDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CouponDto>({
      method: 'POST',
      url: '/api/app/coupon-admin',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getByCode = (code: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CouponDto>({
      method: 'GET',
      url: `/api/app/coupon-admin/by-code/${code}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: PagedAndSortedResultRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<CouponDto>>({
      method: 'GET',
      url: '/api/app/coupon-admin',
      params: { sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
}