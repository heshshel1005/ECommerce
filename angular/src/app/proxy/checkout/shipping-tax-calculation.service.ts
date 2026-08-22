import type { ShippingOptionDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class ShippingTaxCalculationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  calculateTax = (subtotal: number, countryCode?: string, regionCode?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, number>({
      method: 'POST',
      url: '/api/app/shipping-tax-calculation/calculate-tax',
      params: { subtotal, countryCode, regionCode },
    },
    { apiName: this.apiName,...config });
  

  getShippingOptions = (subtotal: number, countryCode?: string, regionCode?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ShippingOptionDto[]>({
      method: 'GET',
      url: '/api/app/shipping-tax-calculation/shipping-options',
      params: { subtotal, countryCode, regionCode },
    },
    { apiName: this.apiName,...config });
}