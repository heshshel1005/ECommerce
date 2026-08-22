import type { CreateUpdateCustomerAddressDto, CustomerAddressDto, CustomerProfileDto, UpdateCustomerProfileDto } from './models';
import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class CustomerProfileService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  createAddress = (input: CreateUpdateCustomerAddressDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerAddressDto>({
      method: 'POST',
      url: '/api/app/customer-profile/address',
      body: input,
    },
    { apiName: this.apiName,...config });
  

  deleteAddress = (id: string, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'DELETE',
      url: `/api/app/customer-profile/${id}/address`,
    },
    { apiName: this.apiName,...config });
  

  getMyAddresses = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerAddressDto[]>({
      method: 'GET',
      url: '/api/app/customer-profile/my-addresses',
    },
    { apiName: this.apiName,...config });
  

  getMyProfile = (config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerProfileDto>({
      method: 'GET',
      url: '/api/app/customer-profile/my-profile',
    },
    { apiName: this.apiName,...config });
  

  updateAddress = (id: string, input: CreateUpdateCustomerAddressDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerAddressDto>({
      method: 'PUT',
      url: `/api/app/customer-profile/${id}/address`,
      body: input,
    },
    { apiName: this.apiName,...config });
  

  updateMyProfile = (input: UpdateCustomerProfileDto, config?: Partial<Rest.Config>) =>
    this.restService.request<any, CustomerProfileDto>({
      method: 'PUT',
      url: '/api/app/customer-profile/my-profile',
      body: input,
    },
    { apiName: this.apiName,...config });
}