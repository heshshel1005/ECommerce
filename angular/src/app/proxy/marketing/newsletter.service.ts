import type { NewsletterSubscriptionStatusDto, SubscribeNewsletterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NewsletterService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getMyStatus = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, NewsletterSubscriptionStatusDto>({
      method: 'GET',
      url: '/api/app/newsletter/my-status',
    },
    { apiName: this.apiName,...config });
  

  subscribe = (input: SubscribeNewsletterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/newsletter/subscribe',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  unsubscribe = (email?: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/newsletter/unsubscribe',
      params: { email },
    },
    { apiName: this.apiName,...config });
}