import type { ConfirmEmailInputDto, CustomerRegisterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { IdentityUserDto } from '../volo/abp/identity/models';

@Injectable({
  providedIn: 'root',
})
export class AccountService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  confirmEmail = (input: ConfirmEmailInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/account/confirm-email',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  subscribe = (input: CustomerRegisterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IdentityUserDto>({
      method: 'POST',
      url: '/api/account/subscribe',
      body: input,
    },
    { apiName: this.apiName,...config });
}