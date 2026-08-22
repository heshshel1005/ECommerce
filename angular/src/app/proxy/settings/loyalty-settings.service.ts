import type { LoyaltySettingsDto, UpdateLoyaltySettingsDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LoyaltySettingsService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  get = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, LoyaltySettingsDto>({
      method: 'GET',
      url: '/api/app/loyalty-settings',
    },
    { apiName: this.apiName,...config });
  

  update = (input: UpdateLoyaltySettingsDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/loyalty-settings',
      body: input,
    },
    { apiName: this.apiName,...config });
}