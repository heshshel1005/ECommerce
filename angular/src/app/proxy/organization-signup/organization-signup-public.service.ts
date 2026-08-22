import type { OrganizationSignupLogoUploadDto, OrganizationSignupSubmitDto, OrganizationSignupSubmitResultDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';
import type { IFormFile } from '../microsoft/asp-net-core/http/models';

@Injectable({
  providedIn: 'root',
})
export class OrganizationSignupPublicService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  submit = (input: OrganizationSignupSubmitDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrganizationSignupSubmitResultDto>({
      method: 'POST',
      url: '/api/app/organization-signup-public/submit',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  uploadLogo = (file: IFormFile, config?: Partial<Rest.Config>) =>
    this.restService.request<any, OrganizationSignupLogoUploadDto>({
      method: 'POST',
      url: '/api/app/organization-signup-public/upload-logo',
      body: file,
    },
    { apiName: this.apiName,...config });
}