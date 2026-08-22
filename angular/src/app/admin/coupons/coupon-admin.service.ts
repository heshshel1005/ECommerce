import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface CouponDto {
  id: string;
  code: string;
  type: number;
  value: number;
  minOrderAmount: number;
  validFrom?: string | null;
  validTo?: string | null;
  totalUsageLimit?: number | null;
  perUserUsageLimit?: number | null;
  isActive: boolean;
}

export interface CreateCouponDto {
  code: string;
  type: number;
  value: number;
  minOrderAmount: number;
  validFrom?: string | null;
  validTo?: string | null;
  totalUsageLimit?: number | null;
  perUserUsageLimit?: number | null;
  isActive: boolean;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface PagedAndSortedResultRequestDto {
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

@Injectable({ providedIn: 'root' })
export class CouponAdminService {
  private readonly rest = inject(RestService);

  getList(params: PagedAndSortedResultRequestDto = {}): Observable<PagedResultDto<CouponDto>> {
    const requestParams: Record<string, string | number | undefined> = {};
    if (params.sorting != null) requestParams.Sorting = params.sorting ?? 'Code';
    if (params.skipCount != null) requestParams.SkipCount = String(params.skipCount);
    if (params.maxResultCount != null) requestParams.MaxResultCount = String(params.maxResultCount ?? 20);
    return this.rest.request<void, PagedResultDto<CouponDto>>({
      method: 'GET',
      url: '/api/app/coupon-admin',
      params: requestParams,
    });
  }

  create(body: CreateCouponDto): Observable<CouponDto> {
    return this.rest.request<CreateCouponDto, CouponDto>({
      method: 'POST',
      url: '/api/app/coupon-admin',
      body,
    });
  }
}
