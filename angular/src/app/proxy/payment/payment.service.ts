import type { ConfirmPaymentRequestDto, ConfirmPaymentResult, CreatePaymentIntentRequestDto, CreatePaymentIntentResult, PaymentGatewayDto, RefundPaymentRequestDto, RefundPaymentResult } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class PaymentService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  confirmPayment = (input: ConfirmPaymentRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, ConfirmPaymentResult>({
      method: 'POST',
      url: '/api/app/payment/confirm',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  createPaymentIntent = (input: CreatePaymentIntentRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CreatePaymentIntentResult>({
      method: 'POST',
      url: '/api/app/payment/create-intent',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  getGateways = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, PaymentGatewayDto[]>({
      method: 'GET',
      url: '/api/app/payment/gateways',
    },
    { apiName: this.apiName,...config });
  

  refund = (input: RefundPaymentRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, RefundPaymentResult>({
      method: 'POST',
      url: '/api/app/payment/refund',
      body: input,
    },
    { apiName: this.apiName,...config });
}