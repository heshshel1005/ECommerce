import type { CreateRedemptionRuleDto, RedemptionRuleDto, UpdateRedemptionRuleDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class RedemptionRuleAdminService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  create = (input: CreateRedemptionRuleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RedemptionRuleDto>({
      method: 'POST',
      url: '/api/app/redemption-rule-admin',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/redemption-rule-admin/${id}`,
    },
    { apiName: this.apiName,...config });
  

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RedemptionRuleDto>({
      method: 'GET',
      url: `/api/app/redemption-rule-admin/${id}`,
    },
    { apiName: this.apiName,...config });
  

  getList = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RedemptionRuleDto[]>({
      method: 'GET',
      url: '/api/app/redemption-rule-admin',
    },
    { apiName: this.apiName,...config });
  

  update = (id: string, input: UpdateRedemptionRuleDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RedemptionRuleDto>({
      method: 'PUT',
      url: `/api/app/redemption-rule-admin/${id}`,
      body: input,
    },
    { apiName: this.apiName,...config });
}