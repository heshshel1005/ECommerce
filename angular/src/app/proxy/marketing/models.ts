import type { EntityDto, PagedResultRequestDto } from '@abp/ng.core';
import type { CouponType } from './coupon-type.enum';

export interface AddGiftRegistryItemDto {
  productVariantId?: string;
  desiredQuantity?: number;
  note?: string | null;
}

export interface ClaimRegistryItemDto {
  giftRegistryItemId?: string;
  quantity?: number;
  claimantName?: string | null;
  message?: string | null;
  addToCart?: boolean;
}

export interface CouponDto extends EntityDto<string> {
  code?: string;
  type?: number;
  value?: number;
  minOrderAmount?: number;
  validFrom?: string | null;
  validTo?: string | null;
  totalUsageLimit?: number | null;
  perUserUsageLimit?: number | null;
  isActive?: boolean;
}

export interface CreateCouponDto {
  code: string;
  type?: CouponType;
  value?: number;
  minOrderAmount?: number;
  validFrom?: string | null;
  validTo?: string | null;
  totalUsageLimit?: number | null;
  perUserUsageLimit?: number | null;
  isActive?: boolean;
}

export interface CreateGiftRegistryDto {
  title: string;
  slug: string;
  eventDate?: string | null;
}

export interface CreateRedemptionRuleDto {
  name?: string;
  type?: number;
  pointsRequired?: number;
  value?: number;
  minOrderAmount?: number;
}

export interface CustomerPointsDto extends EntityDto<string> {
  userId?: string;
  balance?: number;
  tier?: string | null;
}

export interface GiftRegistryDto {
  id?: string;
  title?: string;
  slug?: string;
  eventDate?: string | null;
  items?: GiftRegistryItemDto[];
}

export interface GiftRegistryItemDto {
  id?: string;
  productVariantId?: string;
  productId?: string;
  productName?: string;
  sku?: string;
  price?: number | null;
  desiredQuantity?: number;
  quantityClaimed?: number;
  quantityRemaining?: number;
  note?: string | null;
}

export interface NewsletterSubscriberDto extends EntityDto<string> {
  email?: string;
  name?: string | null;
  isActive?: boolean;
  creationTime?: string;
  unsubscribedAt?: string | null;
}

export interface NewsletterSubscriberListRequestDto extends PagedResultRequestDto {
  isActiveOnly?: boolean | null;
}

export interface NewsletterSubscriptionStatusDto {
  isSubscribed?: boolean;
}

export interface RedemptionRuleDto extends EntityDto<string> {
  name?: string;
  type?: number;
  pointsRequired?: number;
  value?: number;
  minOrderAmount?: number;
  isActive?: boolean;
}

export interface SendNewsletterCampaignDto {
  subject?: string;
  body?: string;
  isBodyHtml?: boolean;
}

export interface SubscribeNewsletterDto {
  email?: string;
  name?: string | null;
}

export interface UpdateRedemptionRuleDto {
  name?: string;
  pointsRequired?: number;
  value?: number;
  minOrderAmount?: number;
  isActive?: boolean;
}

export interface WishlistDto {
  id?: string;
  items?: WishlistItemDto[];
}

export interface WishlistItemDto {
  id?: string;
  productVariantId?: string;
  productId?: string;
  productName?: string;
  sku?: string;
  price?: number | null;
  availableQuantity?: number | null;
}
