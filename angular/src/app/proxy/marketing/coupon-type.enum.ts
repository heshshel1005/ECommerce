import { mapEnumToOptions } from '@abp/ng.core';

export enum CouponType {
  Percent = 0,
  FixedAmount = 1,
}

export const couponTypeOptions = mapEnumToOptions(CouponType);
