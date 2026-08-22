import type { OrganizationSignupRequestDto, OrganizationSignupRequestListRequestDto, RejectOrganizationSignupDto, RepairTenantAdminPermissionsResultDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class OrganizationSignupHostService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  approve = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/organization-signup-host/${id}/approve`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrganizationSignupRequestDto>({
      method: 'GET',
      url: `/api/app/organization-signup-host/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (input: OrganizationSignupRequestListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<OrganizationSignupRequestDto>>({
      method: 'GET',
      url: '/api/app/organization-signup-host',
      params: { status: input.status, sorting: input.sorting, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  reject = (id: string, input: RejectOrganizationSignupDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/organization-signup-host/${id}/reject`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  repairTenantAdminPermissions = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RepairTenantAdminPermissionsResultDto>({
      method: 'POST',
      url: '/api/app/organization-signup-host/repair-tenant-admin-permissions',
    },
    { apiName: this.apiName,...config });
}