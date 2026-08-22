import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { AuthService, LocalizationPipe, SessionStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { Subject } from 'rxjs';
import { distinctUntilChanged, skip, takeUntil } from 'rxjs/operators';
import { BreadcrumbService } from '../../shared/breadcrumbs/breadcrumb.service';
import {
  PublicCatalogService,
  ProductDto,
  ProductVariantDto,
  ProductMediaItemDto,
  ProductMediaType,
} from '../public-catalog.service';
import { WishlistService } from '../wishlist.service';
import { CompareService } from '../compare.service';
import { CartService } from '../cart.service';
import {
  ProductReviewService,
  ProductReviewDto,
  ProductReviewAggregateDto,
} from './product-review.service';

@Component({
  selector: 'app-storefront-product-detail',
  standalone: true,
  imports: [DecimalPipe, RouterLink, LocalizationPipe],
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss'],
})
export class StorefrontProductDetailComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly catalog = inject(PublicCatalogService);
  private readonly toaster = inject(ToasterService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly sessionState = inject(SessionStateService);
  private readonly reviewService = inject(ProductReviewService);
  private readonly wishlistService = inject(WishlistService);
  private readonly cartService = inject(CartService);
  protected readonly compareService = inject(CompareService);
  protected readonly authService = inject(AuthService);
  private readonly destroy$ = new Subject<void>();

  product = signal<ProductDto | null>(null);
  loading = signal(true);
  selectedVariant = signal<ProductVariantDto | null>(null);
  quantity = signal(1);
  /** Gallery: index of selected media (image or video). */
  selectedMediaIndex = signal(0);
  /** Lightbox zoom open. */
  lightboxOpen = signal(false);
  readonly mediaTypeImage: ProductMediaType = 0;
  readonly mediaTypeVideo: ProductMediaType = 1;

  /** Reviews: aggregate and list for PDP */
  reviewAggregate = signal<ProductReviewAggregateDto | null>(null);
  reviewList = signal<ProductReviewDto[]>([]);
  reviewTotalCount = signal(0);
  reviewLoading = signal(false);
  submitReviewLoading = signal(false);
  reviewRating = signal(5);
  reviewText = signal('');
  reviewSkip = signal(0);
  readonly reviewPageSize = 5;
  readonly maxReviewRating = 5;

  /** Max quantity the user can add (selected variant's availableQuantity, or 0 if none). */
  maxQuantity = computed(() => {
    const v = this.selectedVariant();
    if (!v) return 0;
    return Math.max(0, v.availableQuantity ?? 0);
  });

  /** Whether Add to cart should be enabled (variant selected and in stock, quantity in range). */
  canAddToCart = computed(() => {
    const v = this.selectedVariant();
    const q = this.quantity();
    if (!v || (v.availableQuantity ?? 0) <= 0) return false;
    return q >= 1 && q <= this.maxQuantity();
  });

  /** Effective quantity to use (capped to max). */
  effectiveQuantity = computed(() => {
    const q = this.quantity();
    const max = this.maxQuantity();
    if (max <= 0) return 0;
    return Math.min(Math.max(1, q), max);
  });

  ngOnInit(): void {
    this.sessionState
      .getLanguage$()
      .pipe(skip(1), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => this.loadProduct());

    this.loadProduct();
  }

  private loadProduct(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading.set(false);
      return;
    }
    this.loading.set(true);
    this.catalog.getProductDetail(id).subscribe({
      next: (res) => {
        const raw = res != null && typeof res === 'object' ? (res as unknown as Record<string, unknown>) : {};
        const p = (raw.result ?? raw.body ?? raw) as Record<string, unknown> | undefined;
        const normalized = p ? this.normalizeProductDto(p) : null;
        this.product.set(normalized);
        this.selectedMediaIndex.set(0);
        const variants = normalized?.variants ?? [];
        if (variants.length === 1) this.selectedVariant.set(variants[0]);
        this.breadcrumbService.setItems([
          { label: 'ECommerce::Catalog', route: '/catalog' },
          { label: normalized?.name ?? 'Product' },
        ]);
        this.loading.set(false);
        if (id) this.loadReviews(id);
        if (this.authService.isAuthenticated) {
          this.wishlistService.getList().subscribe();
        }
      },
      error: () => {
        this.toaster.error('ECommerce::ProductNotFound', 'Error');
        this.loading.set(false);
      },
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.breadcrumbService.clear();
  }

  selectVariant(v: ProductVariantDto): void {
    this.selectedVariant.set(v);
    const max = Math.max(0, v.availableQuantity ?? 0);
    this.quantity.set(max > 0 ? 1 : 0);
  }

  setQuantity(q: number): void {
    const max = this.maxQuantity();
    if (max <= 0) return;
    this.quantity.set(Math.min(Math.max(1, q), max));
  }

  addingToCart = signal(false);

  addToCart(): void {
    if (!this.canAddToCart()) return;
    const v = this.selectedVariant();
    const q = this.effectiveQuantity();
    if (!v || q <= 0) return;
    this.addingToCart.set(true);
    this.cartService.addItem(v.id, q).subscribe({
      next: () => {
        this.addingToCart.set(false);
        this.toaster.success('ECommerce::AddToCart');
      },
      error: (err) => {
        this.addingToCart.set(false);
        const msg = err?.error?.error?.message ?? err?.message ?? 'ECommerce::ErrorLoadingProducts';
        this.toaster.error(msg, 'ECommerce::Cart');
      },
    });
  }

  isVariantInStock(v: ProductVariantDto): boolean {
    return (v.availableQuantity ?? 0) > 0;
  }

  /** Ordered list of media (images and videos) for gallery. */
  getMediaList(): ProductMediaItemDto[] {
    const p = this.product();
    if (p?.media?.length) return p.media;
    if (p?.mediaIds?.length)
      return p.mediaIds.map((id) => ({ id, mediaType: 0 as ProductMediaType }));
    if (p?.primaryMediaId)
      return [{ id: p.primaryMediaId, mediaType: 0 as ProductMediaType }];
    return [];
  }

  getMediaFileUrl(mediaId: string): string {
    return this.catalog.getMediaFileUrl(mediaId);
  }

  getPrimaryImageUrl(): string | null {
    const p = this.product();
    if (!p?.primaryMediaId) return null;
    return this.catalog.getMediaFileUrl(p.primaryMediaId);
  }

  openLightbox(): void {
    const list = this.getMediaList();
    const idx = this.selectedMediaIndex();
    if (list[idx]?.mediaType === this.mediaTypeImage) this.lightboxOpen.set(true);
  }

  closeLightbox(): void {
    this.lightboxOpen.set(false);
  }

  isInWishlist(): boolean {
    const p = this.product();
    return p?.id ? this.wishlistService.isInWishlist(p.id) : false;
  }

  toggleWishlist(): void {
    const p = this.product();
    if (!p?.id) return;
    if (this.authService.isAuthenticated) {
      const v = this.selectedVariant();
      if (v) {
        if (this.wishlistService.isInWishlist(p.id)) {
          const serverList = this.wishlistService.serverWishlist();
          const item = serverList?.items?.find((i) => i.productId === p.id);
          if (item) {
            this.wishlistService.removeItem(item.id).subscribe({
              next: () => this.toaster.success('ECommerce::RemoveFromWishlist'),
            });
          }
          return;
        }
        this.wishlistService.addVariant(v.id).subscribe({
          next: () => this.toaster.success('ECommerce::AddToWishlist'),
          error: () => this.toaster.error('ECommerce::ErrorLoadingProducts'),
        });
      } else {
        this.toaster.warn('ECommerce::SelectVariant');
      }
      return;
    }
    const added = this.wishlistService.toggle(p.id);
    this.toaster.success(added ? 'ECommerce::AddToWishlist' : 'ECommerce::RemoveFromWishlist');
  }

  addToCompare(): void {
    const p = this.product();
    if (!p?.id) return;
    const ok = this.compareService.add(p.id);
    if (ok) {
      this.toaster.success('ECommerce::AddToCompare');
      const ids = this.compareService.getProductIds();
      this.router.navigate(['/catalog/compare'], { queryParams: ids.length ? { ids: ids.join(',') } : {} });
    } else {
      this.toaster.warn('ECommerce::CompareProducts'); // max 4
    }
  }

  /** Compare list ids (reactive) for template. */
  compareProductIds = computed(() => this.compareService.compareIds() ?? []);

  getVariantAttributeSummary(v: ProductVariantDto): string {
    if (!v.attributes?.length) return '';
    return v.attributes.map((a) => a.value).join(', ');
  }

  loadReviews(productId: string): void {
    this.reviewLoading.set(true);
    this.reviewService.getAggregate(productId).subscribe({
      next: (res) => {
        const agg = this.normalizeAggregate(res);
        this.reviewAggregate.set(agg);
        this.reviewLoading.set(false);
      },
      error: () => this.reviewLoading.set(false),
    });
    this.reviewSkip.set(0);
    this.reviewService
      .getList(productId, { skipCount: 0, maxResultCount: this.reviewPageSize, sorting: 'CreationTime DESC' })
      .subscribe({
        next: (res) => {
          const raw = res as unknown as Record<string, unknown>;
          const list = this.normalizeReviewList(res);
          this.reviewList.set(list);
          const total = (raw?.totalCount ?? raw?.TotalCount) as number | undefined;
          this.reviewTotalCount.set(total ?? list.length);
        },
      });
  }

  loadMoreReviews(): void {
    const p = this.product();
    if (!p?.id) return;
    const skip = this.reviewSkip() + this.reviewPageSize;
    this.reviewSkip.set(skip);
    this.reviewService
      .getList(p.id, { skipCount: skip, maxResultCount: this.reviewPageSize, sorting: 'CreationTime DESC' })
      .subscribe({
        next: (res) => {
          const list = this.normalizeReviewList(res);
          this.reviewList.update((prev) => [...prev, ...list]);
        },
      });
  }

  submitReview(): void {
    const p = this.product();
    if (!p?.id || this.submitReviewLoading()) return;
    const rating = this.reviewRating();
    if (rating < 1 || rating > 5) return;
    this.submitReviewLoading.set(true);
    this.reviewService
      .submit({ productId: p.id, rating, reviewText: this.reviewText() || undefined })
      .subscribe({
        next: () => {
          this.toaster.success('ECommerce::ReviewSubmitted', 'Success');
          this.reviewText.set('');
          this.reviewRating.set(5);
          this.submitReviewLoading.set(false);
          this.loadReviews(p.id);
        },
        error: () => this.submitReviewLoading.set(false),
      });
  }

  setReviewRating(r: number): void {
    this.reviewRating.set(Math.min(this.maxReviewRating, Math.max(1, r)));
  }

  /** Whether there are more reviews to load */
  hasMoreReviews = computed(() => {
    const list = this.reviewList().length;
    const total = this.reviewTotalCount();
    return total > list;
  });

  /** Stars array [1..max] for display */
  reviewStars(agg: ProductReviewAggregateDto | null): number[] {
    if (!agg) return [];
    return Array.from({ length: this.maxReviewRating }, (_, i) => i + 1);
  }

  formatReviewDate(creationTime: string): string {
    if (!creationTime) return '';
    const d = new Date(creationTime);
    return d.toLocaleDateString(undefined, { year: 'numeric', month: 'short', day: 'numeric' });
  }

  private normalizeAggregate(res: unknown): ProductReviewAggregateDto {
    const o = (res && typeof res === 'object' ? res : {}) as Record<string, unknown>;
    const data = (o.result ?? o.body ?? o) as Record<string, unknown>;
    const avg = data?.averageRating ?? data?.AverageRating;
    const total = data?.totalCount ?? data?.TotalCount;
    return {
      averageRating: typeof avg === 'number' && !Number.isNaN(avg) ? avg : 0,
      totalCount: typeof total === 'number' && !Number.isNaN(total) ? total : 0,
    };
  }

  private normalizeReviewList(res: unknown): ProductReviewDto[] {
    const o = (res && typeof res === 'object' ? res : {}) as Record<string, unknown>;
    const items = (o.items ?? o.Items ?? []) as unknown[];
    return items.map((item) => {
      const r = (item && typeof item === 'object' ? item : {}) as Record<string, unknown>;
      return {
        id: (r.id ?? r.Id) as string,
        productId: (r.productId ?? r.ProductId) as string,
        userId: (r.userId ?? r.UserId) as string,
        authorDisplayName: (r.authorDisplayName ?? r.AuthorDisplayName) as string ?? '',
        rating: (r.rating ?? r.Rating) as number ?? 0,
        reviewText: (r.reviewText ?? r.ReviewText) as string | null,
        status: (r.status ?? r.Status) as number,
        creationTime: (r.creationTime ?? r.CreationTime) as string ?? '',
      };
    });
  }

  private normalizeProductDto(p: Record<string, unknown>): ProductDto {
    const variantsRaw = (p.variants ?? p.Variants ?? []) as Record<string, unknown>[];
    const mediaRaw = (p.media ?? p.Media ?? []) as Record<string, unknown>[];
    const media: ProductMediaItemDto[] = mediaRaw.map((m) => ({
      id: (m.id ?? m.Id) as string,
      mediaType: ((m.mediaType ?? m.MediaType) as ProductMediaType) ?? 0,
    }));
    return {
      id: (p.id ?? p.Id) as string,
      productNumber: (p.productNumber ?? p.ProductNumber) as string,
      name: (p.name ?? p.Name) as string,
      description: (p.description ?? p.Description) as string | null,
      categoryId: (p.categoryId ?? p.CategoryId) as string | null,
      categoryName: (p.categoryName ?? p.CategoryName) as string | null,
      brandId: (p.brandId ?? p.BrandId) as string | undefined,
      brandName: (p.brandName ?? p.BrandName) as string | null,
      modelId: (p.modelId ?? p.ModelId) as string | null,
      modelName: (p.modelName ?? p.ModelName) as string | null,
      isPublished: (p.isPublished ?? p.IsPublished) as boolean,
      primaryMediaId: (p.primaryMediaId ?? p.PrimaryMediaId) as string | null,
      mediaIds: ((p.mediaIds ?? p.MediaIds) as string[] | undefined) ?? [],
      media: media.length ? media : undefined,
      variants: variantsRaw.map((v) => {
        const attrsRaw = ((v.attributes ?? v.Attributes) ?? []) as Record<string, unknown>[];
        return {
          id: (v.id ?? v.Id) as string,
          productId: (v.productId ?? v.ProductId) as string,
          sku: (v.sku ?? v.Sku) as string,
          price: (v.price ?? v.Price) as number | null,
          quantity: ((v.quantity ?? v.Quantity) as number) ?? 0,
          reserved: ((v.reserved ?? v.Reserved) as number) ?? 0,
          availableQuantity: ((v.availableQuantity ?? v.AvailableQuantity) as number) ?? 0,
          attributes: attrsRaw.map((a) => ({
            productAttributeId: (a.productAttributeId ?? a.ProductAttributeId) as string,
            productAttributeName: (a.productAttributeName ?? a.ProductAttributeName) as string,
            value: (a.value ?? a.Value) as string,
          })),
        };
      }),
    };
  }
}
