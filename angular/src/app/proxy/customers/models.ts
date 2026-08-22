import type { EntityDto } from '@abp/ng.core';

export interface CreateUpdateCustomerAddressDto {
  label?: string;
  street: string;
  city?: string | null;
  region?: string | null;
  postalCode?: string | null;
  country?: string | null;
  isDefaultShipping?: boolean;
  isDefaultBilling?: boolean;
}

export interface CustomerAddressDto extends EntityDto<string> {
  userId?: string;
  label?: string;
  street?: string;
  city?: string | null;
  region?: string | null;
  postalCode?: string | null;
  country?: string | null;
  isDefaultShipping?: boolean;
  isDefaultBilling?: boolean;
}

export interface CustomerProfileDto extends EntityDto<string> {
  userId?: string;
  displayName?: string;
  phoneNumber?: string | null;
  email?: string | null;
}

export interface UpdateCustomerProfileDto {
  displayName: string;
  phoneNumber?: string | null;
}
