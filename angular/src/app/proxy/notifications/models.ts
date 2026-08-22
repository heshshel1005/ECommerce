export interface UserNotificationDto {
  id: string;
  tenantId?: string;
  userId: string;
  title: string;
  message?: string;
  notificationType?: string;
  linkUrl?: string;
  isRead: boolean;
  readTime?: string;
  notificationDate: string;
  data?: string;
  creationTime: string;
}

export interface NotificationCountDto {
  unreadCount: number;
  totalCount: number;
}

export interface GetNotificationsInput {
  isRead?: boolean;
  notificationType?: string;
  skipCount?: number;
  maxResultCount?: number;
  sorting?: string;
}

export interface CreateNotificationDto {
  userId?: string;
  title: string;
  message?: string;
  notificationType?: string;
  linkUrl?: string;
  data?: string;
}
