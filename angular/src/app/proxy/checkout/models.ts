import type { CartDto } from '../cart/models';

export interface CheckoutAddressDto {
  street: string;
  street2?: string | null;
  city?: string | null;
  region?: string | null;
  postalCode?: string | null;
  country?: string | null;
  deliveryInstructions?: string | null;
}

export interface CheckoutSummaryDto {
  cart?: CartDto;
  subTotal?: number;
  discountAmount?: number;
  appliedCouponCode?: string | null;
  shippingOptions?: ShippingOptionDto[];
  taxAmount?: number;
  defaultShippingMethodCode?: string | null;
}

export interface ShippingOptionDto {
  code?: string;
  name?: string;
  amount?: number;
}

export interface SubmitCheckoutDto {
  contactEmail: string;
  contactPhone?: string | null;
  contactName?: string | null;
  shippingAddress: CheckoutAddressDto;
  billingSameAsShipping?: boolean;
  billingAddress?: CheckoutAddressDto | null;
  shippingMethodCode: string;
  couponCode?: string | null;
}

export interface SubmitCheckoutResultDto {
  orderId?: string;
  status?: string;
}
