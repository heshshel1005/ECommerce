import { Injectable, inject } from '@angular/core';
import { RestService, SessionStateService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface PublicProductListDto {
  id: string;
  productNumber: string;
  name: string;
  categoryId?: string | null;
  categoryName?: string | null;
  priceFrom?: number | null;
  isInStock: boolean;
  primaryMediaId?: string | null;
  brandId: string;
  brandName?: string | null;
  modelId?: string | null;
  modelName?: string | null;
}

export interface PublicProductListRequestDto {
  search?: string;
  categoryId?: string | null;
  priceMin?: number | null;
  priceMax?: number | null;
  productTypeId?: string | null;
  dynamicFiltersJson?: string | null;
  brandId?: string | null;
  modelId?: string | null;
  sorting?: string;
  skipCount?: number;
  maxResultCount?: number;
}

export interface ProductVariantAttributeDto {
  productAttributeId: string;
  productAttributeName: string;
  value: string;
}

export interface ProductVariantDto {
  id: string;
  productId: string;
  sku: string;
  price?: number | null;
  quantity: number;
  reserved: number;
  availableQuantity: number;
  attributes: ProductVariantAttributeDto[];
}

export type ProductMediaType = 0 | 1; // 0 = Image, 1 = Video

export interface ProductMediaItemDto {
  id: string;
  mediaType: ProductMediaType;
}

export interface ProductDto {
  id: string;
  productNumber: string;
  name: string;
  description?: string | null;
  categoryId?: string | null;
  categoryName?: string | null;
  brandId?: string;
  brandName?: string | null;
  modelId?: string | null;
  modelName?: string | null;
  isPublished: boolean;
  /** Primary image media id for PDP; null if none. */
  primaryMediaId?: string | null;
  /** All image media ids for PDP gallery (backward compatibility). */
  mediaIds?: string[];
  /** All media (images and videos) for PDP gallery with type. */
  media?: ProductMediaItemDto[];
  variants: ProductVariantDto[];
}

export interface CategoryTreeDto {
  id: string;
  parentId?: string | null;
  name: string;
  slug?: string | null;
  displayOrder: number;
  children: CategoryTreeDto[];
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

export interface BrandFilterItemDto {
  id: string;
  name: string;
}

export interface ModelFilterItemDto {
  id: string;
  brandId: string;
  name: string;
}

export interface CatalogAttributeFilterValueDto {
  value: string;
  displayName?: string | null;
  displayNameLanguage?: string | null;
  fallbackDisplayName?: string | null;
  fallbackDisplayNameLanguage?: string | null;
}

export interface CatalogAttributeFilterItemDto {
  key: string;
  displayName?: string | null;
  displayNameLanguage?: string | null;
  fallbackDisplayName?: string | null;
  fallbackDisplayNameLanguage?: string | null;
  localizedValues?: CatalogAttributeFilterValueDto[];
  values: string[];
}

export interface CatalogFilterOptionsDto {
  attributes: CatalogAttributeFilterItemDto[];
  brands: BrandFilterItemDto[];
  models: ModelFilterItemDto[];
}

@Injectable({ providedIn: 'root' })
export class PublicCatalogService {
  private readonly rest = inject(RestService);
  private readonly sessionState = inject(SessionStateService);

  getFilterOptions(params?: { categoryId?: string | null; productTypeId?: string | null }): Observable<CatalogFilterOptionsDto> {
    const requestParams: Record<string, string> = {};
    if (params?.categoryId) requestParams.CategoryId = params.categoryId;
    if (params?.productTypeId) requestParams.ProductTypeId = params.productTypeId;
    return this.rest.request<void, CatalogFilterOptionsDto>({
      method: 'GET',
      url: '/api/app/public-catalog/filter-options',
      params: requestParams,
    });
  }

  getProductList(params: PublicProductListRequestDto): Observable<PagedResultDto<PublicProductListDto>> {
    const requestParams: Record<string, string | number | undefined> = {};
    if (params.search != null && params.search !== '') requestParams.Search = params.search;
    if (params.categoryId != null && params.categoryId !== '') requestParams.CategoryId = params.categoryId;
    if (params.priceMin != null) requestParams.PriceMin = String(params.priceMin);
    if (params.priceMax != null) requestParams.PriceMax = String(params.priceMax);
    if (params.productTypeId != null && params.productTypeId !== '') requestParams.ProductTypeId = params.productTypeId;
    if (params.dynamicFiltersJson != null && params.dynamicFiltersJson !== '') requestParams.DynamicFiltersJson = params.dynamicFiltersJson;
    if (params.brandId != null && params.brandId !== '') requestParams.BrandId = params.brandId;
    if (params.modelId != null && params.modelId !== '') requestParams.ModelId = params.modelId;
    if (params.sorting != null) requestParams.Sorting = params.sorting;
    if (params.skipCount != null) requestParams.SkipCount = String(params.skipCount);
    if (params.maxResultCount != null) requestParams.MaxResultCount = String(params.maxResultCount);
    return this.rest.request<void, PagedResultDto<PublicProductListDto>>({
      method: 'GET',
      url: '/api/app/public-catalog/products',
      params: requestParams,
    });
  }

  getProductDetail(id: string): Observable<ProductDto> {
    return this.rest.request<void, ProductDto>({
      method: 'GET',
      url: `/api/app/public-catalog/products/${id}`,
    });
  }

  getCategoryTree(): Observable<CategoryTreeDto[]> {
    return this.rest.request<void, CategoryTreeDto[]>({
      method: 'GET',
      url: '/api/app/public-catalog/categories/tree',
    });
  }

  /** Get products by ids for comparison (max 4). Pass comma-separated ids or array. */
  getCompare(productIds: string[] | string): Observable<ProductDto[]> {
    const ids = typeof productIds === 'string'
      ? productIds.split(',').map((s) => s.trim()).filter(Boolean).slice(0, 4)
      : (productIds ?? []).slice(0, 4);
    if (ids.length === 0) {
      return this.rest.request<void, ProductDto[]>({ method: 'GET', url: '/api/app/public-catalog/compare' });
    }
    return this.rest.request<void, ProductDto[]>({
      method: 'GET',
      url: '/api/app/public-catalog/compare',
      params: { ids: ids.join(',') },
    });
  }

  /** URL for product media file (image/video). Use for catalog cards and PDP. */
  getMediaFileUrl(mediaId: string): string {
    const base = environment?.apis?.default?.url ?? '';
    const tenantId = this.sessionState.getTenant()?.id;
    const tenantQuery = tenantId ? `?__tenant=${encodeURIComponent(tenantId)}` : '';
    if (base) {
      return base.replace(/\/$/, '') + '/api/app/product-media/' + mediaId + '/file' + tenantQuery;
    }
    return '/api/app/product-media/' + mediaId + '/file' + tenantQuery;
  }
}
