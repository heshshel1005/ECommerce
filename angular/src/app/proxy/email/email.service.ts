import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class EmailService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  sendEmail = (to: string, subject: string, body: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/email/send-email',
      params: { to, subject, body },
    },
    { apiName: this.apiName,...config });
  

  sendEmailToUser = (userId: string, subject: string, body: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/email/send-email-to-user/${userId}`,
      params: { subject, body },
    },
    { apiName: this.apiName,...config });
  

  sendEmailToUserName = (userName: string, subject: string, body: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/email/send-email-to-user-name',
      params: { userName, subject, body },
    },
    { apiName: this.apiName,...config });
}