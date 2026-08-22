import { mapEnumToOptions } from '@abp/ng.core';

export enum OrganizationBusinessType {
  General = 0,
  AutoParts = 1,
  Clothing = 2,
  Electronics = 3,
  FoodAndBeverage = 4,
  HomeAndGarden = 5,
  HealthAndBeauty = 6,
  Sports = 7,
  Books = 8,
  Other = 9,
}

export const organizationBusinessTypeOptions = mapEnumToOptions(OrganizationBusinessType);
