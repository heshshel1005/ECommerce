import type { ConfirmEmailInputDto, CustomerRegisterDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { RegisterDto, ResetPasswordDto, SendPasswordResetCodeDto, VerifyPasswordResetTokenInput } from '../volo/abp/account/models';
import type { IdentityUserDto } from '../volo/abp/identity/models';

@Injectable({
  providedIn: 'root',
})
export class ECommerceAccountService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  confirmEmail = (input: ConfirmEmailInputDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/e-commerce-account/confirm-email',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  register = (input: RegisterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IdentityUserDto>({
      method: 'POST',
      url: '/api/app/e-commerce-account/register',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  resetPassword = (input: ResetPasswordDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/e-commerce-account/reset-password',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  sendPasswordResetCode = (input: SendPasswordResetCodeDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/e-commerce-account/send-password-reset-code',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  subscribe = (input: CustomerRegisterDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, IdentityUserDto>({
      method: 'POST',
      url: '/api/app/e-commerce-account/subscribe',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  verifyPasswordResetToken = (input: VerifyPasswordResetTokenInput, config?: Partial<Rest.Config>) =>
    this.restService.request<any, boolean>({
      method: 'POST',
      url: '/api/app/e-commerce-account/verify-password-reset-token',
      body: input,
    },
    { apiName: this.apiName,...config });
}