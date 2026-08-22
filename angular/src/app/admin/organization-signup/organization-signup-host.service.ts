import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface OrganizationSignupRequestDto {
  id: string;
  tenantName: string;
  displayName: string;
  legalName?: string | null;
  businessType: number;
  website?: string | null;
  phone?: string | null;
  shortDescription?: string | null;
  logoFilePathOrKey?: string | null;
  adminEmail: string;
  adminUserName: string;
  adminDisplayName: string;
  status: number;
  rejectionReason?: string | null;
  reviewedTime?: string | null;
  reviewerUserId?: string | null;
  createdTenantId?: string | null;
  creationTime: string;
}

export interface OrganizationSignupRequestListRequestDto {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
  /** 0 = Pending, 1 = Approved, 2 = Rejected; omit for all */
  status?: number;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface RejectOrganizationSignupDto {
  reason: string;
}

export interface RepairTenantAdminPermissionsResultDto {
  totalTenants: number;
  repairedTenants: number;
  failedTenants: number;
}

@Injectable({ providedIn: 'root' })
export class OrganizationSignupHostService {
  private readonly rest = inject(RestService);

  getList(
    params: OrganizationSignupRequestListRequestDto = {},
  ): Observable<PagedResultDto<OrganizationSignupRequestDto>> {
    const requestParams: Record<string, string | number | undefined> = {};
    if (params.skipCount != null) requestParams.SkipCount = params.skipCount;
    if (params.maxResultCount != null) requestParams.MaxResultCount = params.maxResultCount;
    if (params.sorting != null) requestParams.Sorting = params.sorting;
    if (params.status != null) requestParams.Status = params.status;
    return this.rest.request<void, PagedResultDto<OrganizationSignupRequestDto>>({
      method: 'GET',
      url: '/api/app/organization-signup-host',
      params: requestParams,
    });
  }

  get(id: string): Observable<OrganizationSignupRequestDto> {
    return this.rest.request<void, OrganizationSignupRequestDto>({
      method: 'GET',
      url: `/api/app/organization-signup-host/${id}`,
    });
  }

  approve(id: string): Observable<void> {
    return this.rest.request<void, void>({
      method: 'POST',
      url: `/api/app/organization-signup-host/${id}/approve`,
    });
  }

  reject(id: string, input: RejectOrganizationSignupDto): Observable<void> {
    return this.rest.request<RejectOrganizationSignupDto, void>({
      method: 'POST',
      url: `/api/app/organization-signup-host/${id}/reject`,
      body: input,
    });
  }

  repairTenantAdminPermissions(): Observable<RepairTenantAdminPermissionsResultDto> {
    return this.rest.request<void, RepairTenantAdminPermissionsResultDto>({
      method: 'POST',
      url: '/api/app/organization-signup-host/repair-tenant-admin-permissions',
    });
  }
}
