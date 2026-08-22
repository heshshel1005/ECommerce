import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { CustomerRegisterDto } from './models/customer-register.dto';

export interface IdentityUserDto {
  id: string;
  userName?: string;
  email?: string;
  [key: string]: unknown;
}

@Injectable({ providedIn: 'root' })
export class SubscriptionService {
  private readonly rest = inject(RestService);

  subscribe(input: CustomerRegisterDto): Observable<IdentityUserDto> {
    return this.rest.request<CustomerRegisterDto, IdentityUserDto>(
      {
        method: 'POST',
        url: '/api/account/subscribe',
        body: input,
      },
      { apiName: 'AbpAccountPublic' }
    );
  }

  confirmEmail(userId: string, token: string): Observable<void> {
    return this.rest.request<{ userId: string; token: string }, void>(
      {
        method: 'POST',
        url: '/api/account/confirm-email',
        body: { userId, token },
      },
      { apiName: 'AbpAccountPublic' }
    );
  }
}
