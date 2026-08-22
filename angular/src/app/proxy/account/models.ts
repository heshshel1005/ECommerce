
export interface AddressInputDto {
  label?: string;
  street: string;
  city?: string | null;
  region?: string | null;
  postalCode?: string | null;
  country?: string | null;
}

export interface ConfirmEmailInputDto {
  userId?: string;
  token?: string;
}

export interface CustomerRegisterDto {
  userName: string;
  emailAddress: string;
  password: string;
  appName?: string | null;
  returnUrl?: string | null;
  returnUrlHash?: string | null;
  displayName: string;
  phoneNumber?: string | null;
  shippingAddress: AddressInputDto;
  billingAddress?: AddressInputDto | null;
}
