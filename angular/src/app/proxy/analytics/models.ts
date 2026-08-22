
export interface AnalyticsFilterDto {
  dateFrom?: string | null;
  dateTo?: string | null;
}

export interface SalesByDayDto {
  date?: string;
  orderCount?: number;
  revenue?: number;
}

export interface SalesSummaryDto {
  totalOrders?: number;
  totalRevenue?: number;
  periodStart?: string | null;
  periodEnd?: string | null;
}

export interface TopProductDto {
  productId?: string;
  productName?: string;
  quantity?: number;
  revenue?: number;
}
