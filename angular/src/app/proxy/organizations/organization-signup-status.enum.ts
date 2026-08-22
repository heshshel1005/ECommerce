import { mapEnumToOptions } from '@abp/ng.core';

export enum OrganizationSignupStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}

export const organizationSignupStatusOptions = mapEnumToOptions(OrganizationSignupStatus);
