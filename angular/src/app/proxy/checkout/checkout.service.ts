import type { CheckoutSummaryDto, SubmitCheckoutDto, SubmitCheckoutResultDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CheckoutService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getSummary = (guestCartId?: string, couponCode?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CheckoutSummaryDto>({
      method: 'GET',
      url: '/api/app/checkout/summary',
      params: { guestCartId, couponCode },
    },
    { apiName: this.apiName,...config });
  

  submitOrder = (input: SubmitCheckoutDto, guestCartId?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, SubmitCheckoutResultDto>({
      method: 'POST',
      url: '/api/app/checkout/submit',
      params: { guestCartId },
      body: input,
    },
    { apiName: this.apiName,...config });
}