import { mapEnumToOptions } from '@abp/ng.core';

export enum ProductMediaType {
  Image = 0,
  Video = 1,
}

export const productMediaTypeOptions = mapEnumToOptions(ProductMediaType);
