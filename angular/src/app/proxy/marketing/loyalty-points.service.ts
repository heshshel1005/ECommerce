import type { CustomerPointsDto, RedemptionRuleDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoyaltyPointsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getActiveRedemptionRules = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, RedemptionRuleDto[]>({
      method: 'GET',
      url: '/api/app/loyalty/redemption-rules',
    },
    { apiName: this.apiName,...config });
  

  getMyPoints = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerPointsDto>({
      method: 'GET',
      url: '/api/app/loyalty/my-points',
    },
    { apiName: this.apiName,...config });
}