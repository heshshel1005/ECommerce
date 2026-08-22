import { provideAppInitializer, inject } from '@angular/core';
import { NavItemsService, NavItem } from '@abp/ng.theme.shared';
import { OAuthService } from 'angular-oauth2-oidc';
import { filter } from 'rxjs/operators';
import { NotificationComponent } from './notification.component';
import { NotificationSignalrService } from './notification-signalr.service';

function initNotifications() {
  const navItems = inject(NavItemsService);
  const signalrService = inject(NotificationSignalrService);
  const oAuthService = inject(OAuthService);

  navItems.addItems([
    new NavItem({
      id: 'notification-bell',
      component: NotificationComponent,
      order: 5,
    }),
  ]);

  if (oAuthService.hasValidAccessToken()) {
    signalrService.startConnection();
  }

  oAuthService.events
    .pipe(filter(e => e.type === 'token_received' || e.type === 'token_refreshed'))
    .subscribe(() => {
      if (oAuthService.hasValidAccessToken()) {
        signalrService.startConnection();
      }
    });
}

export const NOTIFICATION_PROVIDER = provideAppInitializer(() => initNotifications());
