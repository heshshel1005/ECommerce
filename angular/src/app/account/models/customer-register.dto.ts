export interface AddressInputDto {
  label: string;
  street: string;
  city?: string;
  region?: string;
  postalCode?: string;
  country?: string;
}

export interface CustomerRegisterDto {
  userName: string;
  emailAddress: string;
  password: string;
  appName?: string;
  displayName: string;
  phoneNumber?: string;
  shippingAddress: AddressInputDto;
  billingAddress?: AddressInputDto;
}
