import type { NewsletterSubscriberDto, NewsletterSubscriberListRequestDto, SendNewsletterCampaignDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import type { PagedResultDto } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class NewsletterAdminService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  getSubscribers = (input: NewsletterSubscriberListRequestDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, PagedResultDto<NewsletterSubscriberDto>>({
      method: 'GET',
      url: '/api/app/newsletter-admin/subscribers',
      params: { isActiveOnly: input.isActiveOnly, skipCount: input.skipCount, maxResultCount: input.maxResultCount },
    },
    { apiName: this.apiName,...config });
  

  sendCampaign = (input: SendNewsletterCampaignDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/newsletter-admin/campaign/send',
      body: input,
    },
    { apiName: this.apiName,...config });
}