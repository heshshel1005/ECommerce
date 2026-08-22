import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LocalizationPipe, SessionStateService } from '@abp/ng.core';
import { BreadcrumbService } from '../../shared/breadcrumbs/breadcrumb.service';
import { Subject } from 'rxjs';
import { distinctUntilChanged, skip, takeUntil } from 'rxjs/operators';
import {
  PublicCatalogService,
  ProductDto,
  ProductVariantDto,
} from '../public-catalog.service';
import { CompareService } from '../compare.service';

@Component({
  selector: 'app-storefront-product-compare',
  standalone: true,
  imports: [DecimalPipe, RouterLink, LocalizationPipe],
  templateUrl: './product-compare.component.html',
  styleUrls: ['./product-compare.component.scss'],
})
export class StorefrontProductCompareComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly catalog = inject(PublicCatalogService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly compareService = inject(CompareService);
  private readonly sessionState = inject(SessionStateService);
  private readonly destroy$ = new Subject<void>();

  products = signal<ProductDto[]>([]);
  loading = signal(true);

  /** Price range per product (min variant price). */
  priceRange = computed(() =>
    this.products().map((p) => {
      const prices = p.variants?.filter((v) => v.price != null).map((v) => v.price!);
      return prices?.length ? Math.min(...prices) : null;
    })
  );

  /** All attribute names across products (for comparison rows). */
  attributeNames = computed(() => {
    const set = new Set<string>();
    for (const p of this.products()) {
      for (const v of p.variants ?? []) {
        for (const a of v.attributes ?? []) {
          if (a.productAttributeName) set.add(a.productAttributeName);
        }
      }
    }
    return Array.from(set).sort();
  });

  ngOnInit(): void {
    this.breadcrumbService.setItems([
      { label: 'ECommerce::Catalog', route: '/catalog' },
      { label: 'ECommerce::ProductComparison' },
    ]);
    this.loadProducts();
    this.sessionState
      .getLanguage$()
      .pipe(skip(1), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => this.loadProducts());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.breadcrumbService.clear();
  }

  private loadProducts(): void {
    this.loading.set(true);
    const idsParam = this.route.snapshot.queryParamMap.get('ids');
    const ids = idsParam ? idsParam.split(',').map((s) => s.trim()).filter(Boolean) : this.compareService.getProductIds();
    if (ids.length === 0) {
      this.products.set([]);
      this.loading.set(false);
      return;
    }
    this.catalog.getCompare(ids).subscribe({
      next: (res) => {
        const raw = res as unknown;
        const list = Array.isArray(raw) ? raw : (raw && typeof raw === 'object' && 'items' in raw ? (raw as { items: unknown }).items : []) as unknown[];
        this.products.set(this.normalizeList(list));
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  getPrimaryImageUrl(p: ProductDto): string | null {
    if (!p?.primaryMediaId) return null;
    return this.catalog.getMediaFileUrl(p.primaryMediaId);
  }

  getVariantAttributeValue(v: ProductVariantDto, attrName: string): string | null {
    const a = v.attributes?.find((x) => x.productAttributeName === attrName);
    return a?.value ?? null;
  }

  getProductAttributeValues(p: ProductDto, attrName: string): string[] {
    const values = new Set<string>();
    for (const v of p.variants ?? []) {
      const val = this.getVariantAttributeValue(v, attrName);
      if (val) values.add(val);
    }
    return Array.from(values);
  }

  removeFromCompare(productId: string): void {
    this.compareService.remove(productId);
    this.products.update((list) => list.filter((p) => p.id !== productId));
  }

  private normalizeList(list: unknown[]): ProductDto[] {
    return list.map((item) => this.normalizeProductDto((item && typeof item === 'object' ? item : {}) as Record<string, unknown>));
  }

  private normalizeProductDto(p: Record<string, unknown>): ProductDto {
    const variantsRaw = (p.variants ?? p.Variants ?? []) as Record<string, unknown>[];
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
