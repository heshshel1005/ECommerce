import { mapEnumToOptions } from '@abp/ng.core';

export enum ProductReviewStatus {
  Pending = 0,
  Approved = 1,
  Rejected = 2,
}

export const productReviewStatusOptions = mapEnumToOptions(ProductReviewStatus);
