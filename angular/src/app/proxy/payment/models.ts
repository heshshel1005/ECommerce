
export interface ConfirmPaymentRequestDto {
  orderId?: string;
  gatewayPaymentId?: string;
}

export interface ConfirmPaymentResult {
  success?: boolean;
  errorCode?: string | null;
  errorMessage?: string | null;
}

export interface CreatePaymentIntentRequestDto {
  orderId?: string;
  gatewayName?: string;
}

export interface CreatePaymentIntentResult {
  success?: boolean;
  errorCode?: string | null;
  errorMessage?: string | null;
  clientSecret?: string | null;
  gatewayPaymentId?: string | null;
  publishableKeyOrClientId?: string | null;
}

export interface PaymentGatewayDto {
  name?: string;
  displayName?: string;
  publishableKeyOrClientId?: string | null;
}

export interface RefundPaymentRequestDto {
  orderId?: string;
  amount?: number | null;
  reason?: string | null;
}

export interface RefundPaymentResult {
  success?: boolean;
  errorCode?: string | null;
  errorMessage?: string | null;
}
