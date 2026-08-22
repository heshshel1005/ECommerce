import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';

export interface CreateShipmentDto {
  carrier?: string | null;
  trackingNumber?: string | null;
  notes?: string | null;
}

export interface OrderDto {
  id?: string;
  status?: string;
  contactEmail?: string;
  contactPhone?: string | null;
  contactName?: string | null;
  shippingStreet?: string;
  shippingStreet2?: string | null;
  shippingCity?: string | null;
  shippingRegion?: string | null;
  shippingPostalCode?: string | null;
  shippingCountry?: string | null;
  shippingMethodName?: string | null;
  subTotal?: number;
  shippingAmount?: number;
  taxAmount?: number;
  total?: number;
  paymentStatus?: string;
  paymentGateway?: string | null;
  externalPaymentId?: string | null;
  creationTime?: string;
  lines?: OrderLineDto[];
  statusHistory?: OrderStatusHistoryDto[];
}

export interface OrderLineDto {
  id?: string;
  productVariantId?: string;
  productId?: string;
  productName?: string;
  sku?: string;
  unitPrice?: number;
  quantity?: number;
  lineTotal?: number;
}

export interface OrderListDto {
  id?: string;
  status?: string;
  paymentStatus?: string;
  contactEmail?: string;
  contactName?: string | null;
  total?: number;
  creationTime?: string;
  userId?: string | null;
}

export interface OrderListRequestDto extends PagedAndSortedResultRequestDto {
  status?: string | null;
  dateFrom?: string | null;
  dateTo?: string | null;
  search?: string | null;
}

export interface OrderStatusHistoryDto {
  id?: string;
  orderId?: string;
  status?: string;
  creationTime?: string;
}

export interface RefundOrderResultDto {
  success?: boolean;
  errorCode?: string | null;
  errorMessage?: string | null;
}

export interface ShipmentDto {
  id?: string;
  orderId?: string;
  carrier?: string | null;
  trackingNumber?: string | null;
  shippedAt?: string | null;
  notes?: string | null;
  creationTime?: string;
}

export interface UpdateOrderStatusDto {
  status?: string;
  trackingNumber?: string | null;
  carrier?: string | null;
}
