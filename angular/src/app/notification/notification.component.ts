import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../proxy/notifications/notification.service';
import type { UserNotificationDto } from '../proxy/notifications';
import { NotificationSignalrService } from './notification-signalr.service';
import { Subject } from 'rxjs';
import { takeUntil, map } from 'rxjs/operators';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent implements OnInit, OnDestroy {
  notifications: UserNotificationDto[] = [];
  unreadCount = 0;
  totalCount = 0;
  isLoading = false;
  showDropdown = false;
  private destroy$ = new Subject<void>();

  constructor(
    private notificationService: NotificationService,
    private signalrService: NotificationSignalrService
  ) {}

  ngOnInit(): void {
    this.loadNotifications();
    this.loadUnreadCount();
    this.subscribeToRealTimeNotifications();
    setInterval(() => this.loadUnreadCount(), 30000);
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadNotifications(): void {
    this.isLoading = true;
    this.notificationService
      .getList({ maxResultCount: 20 })
      .pipe(
        map(r => r.items ?? []),
        takeUntil(this.destroy$)
      )
      .subscribe({
        next: notifications => {
          this.notifications = notifications;
          this.isLoading = false;
        },
        error: err => {
          console.error('Error loading notifications:', err);
          this.isLoading = false;
        },
      });
  }

  loadUnreadCount(): void {
    this.notificationService
      .getUnreadCount()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: count => {
          this.unreadCount = count.unreadCount;
          this.totalCount = count.totalCount;
        },
        error: err => console.error('Error loading unread count:', err),
      });
  }

  subscribeToRealTimeNotifications(): void {
    this.signalrService.notificationReceived$
      .pipe(takeUntil(this.destroy$))
      .subscribe(notification => {
        this.notifications.unshift(notification as any);
        this.unreadCount++;
        this.totalCount++;
        this.showBrowserNotification(notification);
      });
  }

  toggleDropdown(): void {
    this.showDropdown = !this.showDropdown;
    if (this.showDropdown) this.loadNotifications();
  }

  markAsRead(notification: UserNotificationDto): void {
    if (notification.isRead) return;
    this.notificationService
      .markAsRead(notification.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          notification.isRead = true;
          notification.readTime = new Date().toISOString();
          if (this.unreadCount > 0) this.unreadCount--;
        },
        error: err => console.error('Error marking notification as read:', err),
      });
  }

  markAllAsRead(): void {
    this.notificationService
      .markAllAsRead()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notifications.forEach(n => {
            n.isRead = true;
            n.readTime = new Date().toISOString();
          });
          this.unreadCount = 0;
        },
        error: err => console.error('Error marking all as read:', err),
      });
  }

  deleteNotification(notification: UserNotificationDto): void {
    this.notificationService
      .delete(notification.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notifications = this.notifications.filter(n => n.id !== notification.id);
          if (!notification.isRead) this.unreadCount--;
          this.totalCount--;
        },
        error: err => console.error('Error deleting notification:', err),
      });
  }

  deleteAllNotifications(): void {
    if (!confirm('Are you sure you want to delete all notifications?')) return;
    this.notificationService
      .deleteAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.notifications = [];
          this.unreadCount = 0;
          this.totalCount = 0;
        },
        error: err => console.error('Error deleting all notifications:', err),
      });
  }

  getNotificationIcon(type?: string): string {
    switch (type?.toLowerCase()) {
      case 'success': return 'check-circle';
      case 'warning': return 'exclamation-triangle';
      case 'error':   return 'times-circle';
      default:        return 'info-circle';
    }
  }

  getNotificationClass(type?: string): string {
    switch (type?.toLowerCase()) {
      case 'success': return 'notification-success';
      case 'warning': return 'notification-warning';
      case 'error':   return 'notification-error';
      default:        return 'notification-info';
    }
  }

  formatDate(dateString: string): string {
    const date = new Date(dateString);
    const diffMs = Date.now() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1)   return 'Just now';
    if (diffMins < 60)  return `${diffMins} minute${diffMins > 1 ? 's' : ''} ago`;
    if (diffHours < 24) return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
    if (diffDays < 7)   return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
    return date.toLocaleDateString();
  }

  navigateToLink(linkUrl?: string): void {
    if (linkUrl) window.location.href = linkUrl;
  }

  private showBrowserNotification(notification: UserNotificationDto): void {
    if ('Notification' in window && Notification.permission === 'granted') {
      new Notification(notification.title, {
        body: notification.message,
        icon: '/favicon.ico',
        tag: notification.id,
      });
    }
  }
}
