import type { PagedAndSortedResultRequestDto } from '@abp/ng.core';
import type { ProductMediaType } from './product-media-type.enum';
import type { ProductReviewStatus } from './product-review-status.enum';

export interface BrandDto {
  id?: string;
  name?: string;
  slug?: string | null;
  description?: string | null;
  isActive?: boolean;
}

export interface BrandFilterItemDto {
  id?: string;
  name?: string;
}

export interface BrandModelDto {
  id?: string;
  brandId?: string;
  name?: string;
  code?: string | null;
  isActive?: boolean;
  translations?: BrandModelTranslationDto[];
}

export interface BrandModelTranslationDto {
  language: string;
  name: string;
}

export interface BrandTranslationDto {
  language: string;
  name: string;
  description?: string | null;
}

export interface CatalogFilterOptionsDto {
  sizes?: string[];
  colors?: string[];
  brands?: BrandFilterItemDto[];
  models?: ModelFilterItemDto[];
}

export interface CategoryDto {
  id?: string;
  parentId?: string | null;
  name?: string;
  slug?: string;
  displayOrder?: number;
  translations?: CategoryTranslationDto[];
}

export interface CategoryTranslationDto {
  language: string;
  name: string;
}

export interface CategoryTreeDto {
  id?: string;
  parentId?: string | null;
  name?: string;
  slug?: string;
  displayOrder?: number;
  children?: CategoryTreeDto[];
}

export interface CreateBrandDto {
  name: string;
  slug?: string | null;
  description?: string | null;
  isActive?: boolean;
  translations?: BrandTranslationDto[];
}

export interface CreateBrandModelDto {
  brandId: string;
  name: string;
  code?: string | null;
  isActive?: boolean;
  translations?: BrandModelTranslationDto[];
}

export interface CreateCategoryDto {
  name: string;
  slug: string;
  parentId?: string | null;
  displayOrder?: number;
  translations?: CategoryTranslationDto[];
}

export interface CreateProductDto {
  productNumber: string;
  name: string;
  description?: string | null;
  categoryId?: string | null;
  brandId: string;
  modelId?: string | null;
  isPublished?: boolean;
  translations?: ProductTranslationDto[];
  variants?: CreateProductVariantDto[];
}

export interface CreateProductReviewDto {
  productId?: string;
  rating?: number;
  reviewText?: string | null;
}

export interface CreateProductVariantDto {
  sku: string;
  price?: number | null;
  quantity?: number;
  attributes?: ProductVariantAttributeInputDto[];
}

export interface InventoryDto {
  id?: string;
  productVariantId?: string;
  productName?: string | null;
  sku?: string | null;
  quantity?: number;
  reserved?: number;
  availableQuantity?: number;
  lowStockThreshold?: number | null;
  isLowStock?: boolean;
}

export interface InventoryListRequestDto extends PagedAndSortedResultRequestDto {
  productVariantId?: string | null;
  lowStockOnly?: boolean;
}

export interface ModelFilterItemDto {
  id?: string;
  brandId?: string;
  name?: string;
}

export interface ProductAttributeDto {
  id?: string;
  name?: string;
}

export interface ProductDto {
  id?: string;
  productNumber?: string;
  name?: string;
  description?: string | null;
  categoryId?: string | null;
  brandId?: string;
  modelId?: string | null;
  brandName?: string;
  modelName?: string | null;
  isPublished?: boolean;
  primaryMediaId?: string | null;
  mediaIds?: string[];
  media?: ProductMediaItemDto[];
  variants?: ProductVariantDto[];
  translations?: ProductTranslationDto[];
}

export interface ProductListDto {
  id?: string;
  productNumber?: string;
  name?: string;
  categoryId?: string | null;
  categoryName?: string | null;
  isPublished?: boolean;
  priceFrom?: number | null;
}

export interface ProductListRequestDto extends PagedAndSortedResultRequestDto {
  filter?: string | null;
  categoryId?: string | null;
  isPublished?: boolean | null;
}

export interface ProductMediaDto {
  id?: string;
  productId?: string;
  mediaType?: ProductMediaType;
  sortOrder?: number;
  isPrimary?: boolean;
  altText?: string | null;
  creationTime?: string;
}

export interface ProductMediaItemDto {
  id?: string;
  mediaType?: ProductMediaType;
}

export interface ProductReviewAggregateDto {
  averageRating?: number;
  totalCount?: number;
}

export interface ProductReviewDto {
  id?: string;
  productId?: string;
  userId?: string;
  authorDisplayName?: string;
  rating?: number;
  reviewText?: string | null;
  status?: ProductReviewStatus;
  creationTime?: string;
}

export interface ProductReviewListRequestDto extends PagedAndSortedResultRequestDto {
  productId?: string | null;
  status?: ProductReviewStatus | null;
}

export interface ProductTranslationDto {
  language: string;
  name: string;
  description?: string | null;
}

export interface ProductVariantAttributeDto {
  productAttributeId?: string;
  productAttributeName?: string;
  value?: string;
}

export interface ProductVariantAttributeInputDto {
  productAttributeId?: string;
  value?: string;
}

export interface ProductVariantDto {
  id?: string;
  productId?: string;
  sku?: string;
  price?: number | null;
  quantity?: number;
  reserved?: number;
  availableQuantity?: number;
  attributes?: ProductVariantAttributeDto[];
}

export interface PublicProductListDto {
  id?: string;
  productNumber?: string;
  name?: string;
  categoryId?: string | null;
  categoryName?: string | null;
  priceFrom?: number | null;
  isInStock?: boolean;
  primaryMediaId?: string | null;
  brandId?: string;
  brandName?: string | null;
  modelId?: string | null;
  modelName?: string | null;
}

export interface PublicProductListRequestDto extends PagedAndSortedResultRequestDto {
  search?: string | null;
  categoryId?: string | null;
  priceMin?: number | null;
  priceMax?: number | null;
  size?: string | null;
  color?: string | null;
  brandId?: string | null;
  modelId?: string | null;
}

export interface UpdateBrandDto {
  name: string;
  slug?: string | null;
  description?: string | null;
  isActive?: boolean;
  translations?: BrandTranslationDto[];
}

export interface UpdateBrandModelDto {
  brandId: string;
  name: string;
  code?: string | null;
  isActive?: boolean;
  translations?: BrandModelTranslationDto[];
}

export interface UpdateCategoryDto {
  name: string;
  slug: string;
  parentId?: string | null;
  displayOrder?: number;
  translations?: CategoryTranslationDto[];
}

export interface UpdateInventoryDto {
  quantity?: number | null;
  reserved?: number | null;
  lowStockThreshold?: number | null;
}

export interface UpdateProductDto {
  productNumber: string;
  name: string;
  description?: string | null;
  categoryId?: string | null;
  brandId: string;
  modelId?: string | null;
  isPublished?: boolean;
  translations?: ProductTranslationDto[];
  variants?: UpdateProductVariantDto[];
}

export interface UpdateProductMediaDto {
  isPrimary?: boolean;
  sortOrder?: number;
  altText?: string | null;
}

export interface UpdateProductVariantDto {
  id?: string | null;
  sku: string;
  price?: number | null;
  quantity?: number;
  attributes?: ProductVariantAttributeInputDto[];
}
