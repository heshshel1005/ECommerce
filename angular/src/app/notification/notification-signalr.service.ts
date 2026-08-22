import { Injectable, inject } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { Subject, Observable } from 'rxjs';
import { OAuthService } from 'angular-oauth2-oidc';
import { environment } from '../../environments/environment';
import type { UserNotificationDto } from '../proxy/notifications';

@Injectable({ providedIn: 'root' })
export class NotificationSignalrService {
  private hubConnection?: signalR.HubConnection;
  private readonly oAuthService = inject(OAuthService);

  private notificationReceivedSubject = new Subject<UserNotificationDto>();
  public notificationReceived$: Observable<UserNotificationDto> =
    this.notificationReceivedSubject.asObservable();

  get isConnected(): boolean {
    return this.hubConnection?.state === signalR.HubConnectionState.Connected;
  }

  startConnection(): void {
    if (this.isConnected) return;

    const apiUrl = (environment.apis['default'] as { url: string }).url;

    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${apiUrl}/hubs/notification`, {
        accessTokenFactory: () => this.oAuthService.getAccessToken(),
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR connection error:', err));

    this.hubConnection.on('ReceiveNotification', (notification: UserNotificationDto) => {
      this.notificationReceivedSubject.next(notification);
    });
  }

  stopConnection(): void {
    this.hubConnection?.stop();
    this.hubConnection = undefined;
  }
}
