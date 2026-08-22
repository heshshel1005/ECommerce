
export interface AddCartItemDto {
  productVariantId?: string;
  quantity?: number;
}

export interface CartDto {
  id?: string;
  isAuthenticated?: boolean;
  items?: CartItemDto[];
  itemCount?: number;
}

export interface CartItemDto {
  id?: string;
  cartId?: string;
  productVariantId?: string;
  productId?: string;
  productName?: string;
  sku?: string;
  unitPrice?: number | null;
  quantity?: number;
  availableStock?: number | null;
}

export interface MergeGuestCartRequest {
  guestCartId?: string;
}

export interface UpdateCartItemDto {
  quantity?: number;
}
