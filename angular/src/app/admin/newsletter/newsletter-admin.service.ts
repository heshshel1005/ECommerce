import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface NewsletterSubscriberDto {
  id: string;
  email: string;
  name?: string | null;
  isActive: boolean;
  creationTime: string;
  unsubscribedAt?: string | null;
}

export interface NewsletterSubscriberListRequestDto {
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
  isActiveOnly?: boolean;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface SendNewsletterCampaignDto {
  subject: string;
  body: string;
  isBodyHtml: boolean;
}

@Injectable({ providedIn: 'root' })
export class NewsletterAdminService {
  private readonly rest = inject(RestService);

  getSubscribers(
    params: NewsletterSubscriberListRequestDto = {}
  ): Observable<PagedResultDto<NewsletterSubscriberDto>> {
    const requestParams: Record<string, string | number | boolean | undefined> = {};
    if (params.skipCount != null) requestParams.SkipCount = String(params.skipCount);
    if (params.maxResultCount != null)
      requestParams.MaxResultCount = String(params.maxResultCount ?? 20);
    if (params.sorting != null) requestParams.Sorting = params.sorting ?? 'Email';
    if (params.isActiveOnly != null) requestParams.IsActiveOnly = params.isActiveOnly;
    return this.rest.request<void, PagedResultDto<NewsletterSubscriberDto>>({
      method: 'GET',
      url: '/api/app/newsletter-admin/subscribers',
      params: requestParams,
    });
  }

  sendCampaign(dto: SendNewsletterCampaignDto): Observable<void> {
    return this.rest.request<SendNewsletterCampaignDto, void>({
      method: 'POST',
      url: '/api/app/newsletter-admin/campaign/send',
      body: dto,
    });
  }
}
