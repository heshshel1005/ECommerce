import { Injectable, inject } from '@angular/core';
import { RestService, Rest } from '@abp/ng.core';
import type { GetNotificationsInput, NotificationCountDto, UserNotificationDto } from './models';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private restService = inject(RestService);
  apiName = 'Default';

  get = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<void, UserNotificationDto>(
      { method: 'GET', url: `/api/app/notifications/${id}` },
      { apiName: this.apiName, ...config }
    );

  getList = (input: GetNotificationsInput = {}, config?: Partial<Rest.Config>) =>
    this.restService.request<GetNotificationsInput, { items: UserNotificationDto[]; totalCount: number }>(
      { method: 'GET', url: '/api/app/notifications', params: input },
      { apiName: this.apiName, ...config }
    );

  getUnreadCount = (config?: Partial<Rest.Config>) =>
    this.restService.request<void, NotificationCountDto>(
      { method: 'GET', url: '/api/app/notifications/unread-count' },
      { apiName: this.apiName, ...config }
    );

  markAsRead = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<void, void>(
      { method: 'PUT', url: `/api/app/notifications/${id}/mark-as-read` },
      { apiName: this.apiName, ...config }
    );

  markAllAsRead = (config?: Partial<Rest.Config>) =>
    this.restService.request<void, void>(
      { method: 'PUT', url: '/api/app/notifications/mark-all-as-read' },
      { apiName: this.apiName, ...config }
    );

  delete = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<void, void>(
      { method: 'DELETE', url: `/api/app/notifications/${id}` },
      { apiName: this.apiName, ...config }
    );

  deleteAll = (config?: Partial<Rest.Config>) =>
    this.restService.request<void, void>(
      { method: 'DELETE', url: '/api/app/notifications/all' },
      { apiName: this.apiName, ...config }
    );
}
