import {
  Component,
  inject,
  OnInit,
  OnDestroy,
  signal,
  computed,
  effect,
} from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LocalizationPipe, SessionStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { Subject } from 'rxjs';
import { distinctUntilChanged, skip, takeUntil } from 'rxjs/operators';
import { BreadcrumbService } from '../../shared/breadcrumbs/breadcrumb.service';
import {
  PublicCatalogService,
  PublicProductListDto,
  PublicProductListRequestDto,
  CatalogFilterOptionsDto,
  CatalogAttributeFilterItemDto,
} from '../public-catalog.service';
import type { CategoryTreeDto } from '../public-catalog.service';

/** URL query param keys for catalog listing. */
const Q = {
  q: 'q',
  category: 'category',
  priceMin: 'priceMin',
  priceMax: 'priceMax',
  dynamicFilters: 'filters',
  brand: 'brand',
  model: 'model',
  sort: 'sort',
  page: 'page',
  view: 'view',
} as const;

/** Sort option value sent to API (matches backend). */
const SORT_OPTIONS: { value: string; labelKey: string }[] = [
  { value: 'Name', labelKey: 'ECommerce::SortByName' },
  { value: 'Name DESC', labelKey: 'ECommerce::SortByNameDesc' },
  { value: 'PriceFrom', labelKey: 'ECommerce::SortByPrice' },
  { value: 'PriceFrom DESC', labelKey: 'ECommerce::SortByPriceDesc' },
  { value: 'ProductNumber', labelKey: 'ECommerce::SortByProductNumber' },
];

@Component({
  selector: 'app-storefront-product-list',
  standalone: true,
  imports: [DecimalPipe, RouterLink, LocalizationPipe],
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss'],
})
export class StorefrontProductListComponent implements OnInit, OnDestroy {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly catalog = inject(PublicCatalogService);
  private readonly toaster = inject(ToasterService);
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly sessionState = inject(SessionStateService);
  private readonly destroy$ = new Subject<void>();

  readonly sortOptions = SORT_OPTIONS;
  readonly pageSize = 12;

  items = signal<PublicProductListDto[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  search = signal('');
  categoryId = signal<string | null>(null);
  priceMin = signal<number | null>(null);
  priceMax = signal<number | null>(null);
  dynamicFilters = signal<Record<string, string>>({});
  brandId = signal<string | null>(null);
  modelId = signal<string | null>(null);
  sorting = signal('Name');
  page = signal(0);
  viewMode = signal<'grid' | 'list'>('grid');

  categoryTree = signal<CategoryTreeDto[]>([]);
  filterOptions = signal<CatalogFilterOptionsDto>({ attributes: [], brands: [], models: [] });

  categoriesFlat = computed(() => this.flattenCategories(this.categoryTree()));

  /** Models filtered by selected brand (for model dropdown). */
  modelsForSelectedBrand = computed(() => {
    const brandId = this.brandId();
    const opts = this.filterOptions();
    if (!brandId || !opts.models?.length) return opts.models ?? [];
    return opts.models.filter((m) => m.brandId === brandId);
  });

  /** Facet title: current display name → fallback → key. */
  facetLabel(attribute: CatalogAttributeFilterItemDto): string {
    const d = attribute.displayName?.trim();
    if (d) return d;
    const f = attribute.fallbackDisplayName?.trim();
    if (f) return f;
    return attribute.key;
  }

  /** Option label in facet dropdown: localized display → fallback → invariant value. */
  facetOptionLabel(attribute: CatalogAttributeFilterItemDto, invariant: string): string {
    const loc = attribute.localizedValues?.find((v) => v.value === invariant);
    if (loc) {
      const d = loc.displayName?.trim();
      if (d) return d;
      const f = loc.fallbackDisplayName?.trim();
      if (f) return f;
    }
    return invariant;
  }

  skipCount = computed(() => this.page() * this.pageSize);
  totalPages = computed(() =>
    Math.max(1, Math.ceil(this.totalCount() / this.pageSize))
  );

  /** Category name for current categoryId (for breadcrumb). */
  categoryName = computed(() => {
    const id = this.categoryId();
    if (!id) return null;
    return this.findCategoryName(this.categoryTree(), id);
  });

  private findCategoryName(nodes: CategoryTreeDto[], id: string): string | null {
    for (const n of nodes) {
      if (n.id === id) return n.name;
      if (n.children?.length) {
        const found = this.findCategoryName(n.children, id);
        if (found) return found;
      }
    }
    return null;
  }

  private flattenCategories(
    nodes: CategoryTreeDto[],
    level = 0
  ): { id: string; name: string; level: number }[] {
    let out: { id: string; name: string; level: number }[] = [];
    for (const n of nodes) {
      out.push({ id: n.id, name: n.name, level });
      if (n.children?.length)
        out = out.concat(this.flattenCategories(n.children, level + 1));
    }
    return out;
  }

  constructor() {
    effect(() => {
      const items: { label: string; route?: string[] }[] = [
        { label: 'ECommerce::Catalog', route: ['/catalog'] },
      ];
      const catName = this.categoryName();
      const q = this.search().trim();
      if (catName) items.push({ label: catName });
      if (q) items.push({ label: 'ECommerce::SearchResults' });
      this.breadcrumbService.setItems(
        items.map((x, i) => ({
          label: x.label,
          route: i === 0 && x.route?.length ? x.route : undefined,
        }))
      );
    });
  }

  ngOnInit(): void {
    this.reloadSupportingData();

    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe((params) => {
        const q = (params[Q.q] ?? '').trim();
        const category = (params[Q.category] ?? '').trim() || null;
        const priceMin = params[Q.priceMin];
        const priceMax = params[Q.priceMax];
        const dynamicFilters = this.parseDynamicFilters((params[Q.dynamicFilters] ?? '').trim());
        const brand = (params[Q.brand] ?? '').trim() || null;
        const model = (params[Q.model] ?? '').trim() || null;
        const sort = (params[Q.sort] ?? 'Name').trim() || 'Name';
        const page = Math.max(0, parseInt(params[Q.page], 10) || 0);
        const view = params[Q.view] === 'list' ? 'list' : 'grid';

        this.search.set(q);
        this.categoryId.set(category || null);
        this.priceMin.set(priceMin != null ? Number(priceMin) : null);
        this.priceMax.set(priceMax != null ? Number(priceMax) : null);
        this.dynamicFilters.set(dynamicFilters);
        this.brandId.set(brand);
        this.modelId.set(model);
        this.sorting.set(sort);
        this.page.set(page);
        this.viewMode.set(view);
        this.load();
      });

    this.sessionState
      .getLanguage$()
      .pipe(skip(1), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe(() => {
        this.reloadSupportingData();
        this.load();
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.breadcrumbService.clear();
  }

  private reloadSupportingData(): void {
    this.catalog.getCategoryTree().subscribe({
      next: (tree) => this.categoryTree.set(tree),
      error: () => {},
    });

    this.catalog.getFilterOptions({ categoryId: this.categoryId() }).subscribe({
      next: (res) => {
        const raw = res as unknown as Record<string, unknown>;
        const attributesRaw = (raw.attributes ?? raw.Attributes ?? []) as Array<Record<string, unknown>>;
        const brandsRaw = (raw.brands ?? raw.Brands ?? []) as Array<{ id?: string; name?: string }>;
        const modelsRaw = (raw.models ?? raw.Models ?? []) as Array<{ id?: string; brandId?: string; name?: string }>;
        const attributes: CatalogAttributeFilterItemDto[] = attributesRaw.map((a) => {
          const localizedRaw =
            (a['localizedValues'] as CatalogAttributeFilterItemDto['localizedValues']) ??
            (a['LocalizedValues'] as CatalogAttributeFilterItemDto['localizedValues']);
          const localizedValues = Array.isArray(localizedRaw)
            ? localizedRaw.map((v) => ({
                value: String((v as { value?: string }).value ?? ''),
                displayName: (v as { displayName?: string | null }).displayName ?? null,
                displayNameLanguage: (v as { displayNameLanguage?: string | null }).displayNameLanguage ?? null,
                fallbackDisplayName: (v as { fallbackDisplayName?: string | null }).fallbackDisplayName ?? null,
                fallbackDisplayNameLanguage:
                  (v as { fallbackDisplayNameLanguage?: string | null }).fallbackDisplayNameLanguage ?? null,
              }))
            : [];
          return {
            key: String(a['key'] ?? a['Key'] ?? ''),
            displayName: (a['displayName'] ?? a['DisplayName']) as string | null | undefined,
            displayNameLanguage: (a['displayNameLanguage'] ?? a['DisplayNameLanguage']) as string | null | undefined,
            fallbackDisplayName: (a['fallbackDisplayName'] ?? a['FallbackDisplayName']) as string | null | undefined,
            fallbackDisplayNameLanguage: (a['fallbackDisplayNameLanguage'] ?? a['FallbackDisplayNameLanguage']) as
              | string
              | null
              | undefined,
            localizedValues,
            values: Array.isArray(a['values'] ?? a['Values'])
              ? ((a['values'] ?? a['Values']) as string[])
              : [],
          };
        });
        const brands = brandsRaw.map((b) => ({ id: b.id ?? '', name: b.name ?? '' }));
        const models = modelsRaw.map((m) => ({ id: m.id ?? '', brandId: m.brandId ?? '', name: m.name ?? '' }));
        this.filterOptions.set({ attributes, brands, models });
      },
      error: () => {},
    });
  }

  private buildQueryParams(): Record<string, string | number | undefined> {
    const q = this.search().trim();
    const category = this.categoryId();
    const priceMin = this.priceMin();
    const priceMax = this.priceMax();
    const dynamicFilters = this.dynamicFilters();
    const brand = this.brandId();
    const model = this.modelId();
    const sort = this.sorting();
    const page = this.page();
    const view = this.viewMode();

    const params: Record<string, string | number | undefined> = {};
    if (q) params[Q.q] = q;
    if (category) params[Q.category] = category;
    if (priceMin != null && !Number.isNaN(priceMin)) params[Q.priceMin] = priceMin;
    if (priceMax != null && !Number.isNaN(priceMax)) params[Q.priceMax] = priceMax;
    if (Object.keys(dynamicFilters).length > 0) params[Q.dynamicFilters] = JSON.stringify(dynamicFilters);
    if (brand) params[Q.brand] = brand;
    if (model) params[Q.model] = model;
    if (sort && sort !== 'Name') params[Q.sort] = sort;
    if (page > 0) params[Q.page] = page;
    if (view === 'list') params[Q.view] = view;
    return params;
  }

  private updateUrl(): void {
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: this.buildQueryParams(),
      queryParamsHandling: '',
      replaceUrl: true,
    });
  }

  load(): void {
    this.loading.set(true);
    const req: PublicProductListRequestDto = {
      search: this.search().trim() || undefined,
      categoryId: this.categoryId() || undefined,
      priceMin: this.priceMin() ?? undefined,
      priceMax: this.priceMax() ?? undefined,
      dynamicFiltersJson: Object.keys(this.dynamicFilters()).length > 0 ? JSON.stringify(this.dynamicFilters()) : undefined,
      brandId: this.brandId() || undefined,
      modelId: this.modelId() || undefined,
      sorting: this.sorting(),
      skipCount: this.skipCount(),
      maxResultCount: this.pageSize,
    };
    this.catalog.getProductList(req).subscribe({
      next: (res) => {
        const raw =
          res != null && typeof res === 'object'
            ? (res as unknown as Record<string, unknown>)
            : {};
        const data = raw.result ?? raw.body ?? raw;
        const obj =
          data != null && typeof data === 'object'
            ? (data as Record<string, unknown>)
            : {};
        const rawItems = obj.items ?? obj.Items;
        const totalCount = obj.totalCount ?? obj.TotalCount;
        const list = Array.isArray(rawItems)
          ? rawItems.map((item: Record<string, unknown>) => ({
              id: item.id ?? item.Id,
              productNumber: item.productNumber ?? item.ProductNumber,
              name: item.name ?? item.Name,
              categoryId: item.categoryId ?? item.CategoryId,
              categoryName: item.categoryName ?? item.CategoryName,
              priceFrom: item.priceFrom ?? item.PriceFrom,
              isInStock: item.isInStock ?? item.IsInStock ?? false,
              primaryMediaId: item.primaryMediaId ?? item.PrimaryMediaId ?? null,
              brandId: item.brandId ?? item.BrandId ?? '',
              brandName: item.brandName ?? item.BrandName ?? null,
              modelId: item.modelId ?? item.ModelId ?? null,
              modelName: item.modelName ?? item.ModelName ?? null,
            }))
          : [];
        this.items.set(list as PublicProductListDto[]);
        this.totalCount.set(typeof totalCount === 'number' ? totalCount : 0);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.toaster.error('ECommerce::ErrorLoadingProducts', 'Error');
      },
    });
  }

  onSearchChange(): void {
    this.page.set(0);
    this.updateUrl();
  }

  onCategoryChange(categoryId: string | null | undefined): void {
    this.categoryId.set(categoryId && categoryId !== '' ? categoryId : null);
    this.page.set(0);
    this.updateUrl();
  }

  onPriceMinChange(value: string): void {
    const n = value === '' ? null : Number(value);
    this.priceMin.set(n != null && !Number.isNaN(n) ? n : null);
    this.page.set(0);
    this.updateUrl();
  }

  onPriceMaxChange(value: string): void {
    const n = value === '' ? null : Number(value);
    this.priceMax.set(n != null && !Number.isNaN(n) ? n : null);
    this.page.set(0);
    this.updateUrl();
  }

  onDynamicAttributeChange(key: string, value: string | null): void {
    const next = { ...this.dynamicFilters() };
    if (value && value !== '') {
      next[key] = value;
    } else {
      delete next[key];
    }
    this.dynamicFilters.set(next);
    this.page.set(0);
    this.updateUrl();
  }

  onBrandChange(value: string | null): void {
    this.brandId.set(value && value !== '' ? value : null);
    this.modelId.set(null);
    this.page.set(0);
    this.updateUrl();
  }

  onModelChange(value: string | null): void {
    this.modelId.set(value && value !== '' ? value : null);
    this.page.set(0);
    this.updateUrl();
  }

  onSortChange(sorting: string): void {
    this.sorting.set(sorting);
    this.page.set(0);
    this.updateUrl();
  }

  onPageChange(p: number): void {
    this.page.set(p);
    this.updateUrl();
  }

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode.set(mode);
    this.updateUrl();
  }

  clearFilters(): void {
    this.search.set('');
    this.categoryId.set(null);
    this.priceMin.set(null);
    this.priceMax.set(null);
    this.dynamicFilters.set({});
    this.brandId.set(null);
    this.modelId.set(null);
    this.sorting.set('Name');
    this.page.set(0);
    this.updateUrl();
  }

  hasActiveFilters = computed(() => {
    return (
      this.search().trim() !== '' ||
      this.categoryId() != null ||
      this.priceMin() != null ||
      this.priceMax() != null ||
      Object.keys(this.dynamicFilters()).length > 0 ||
      this.brandId() != null ||
      this.modelId() != null
    );
  });

  getDynamicAttributeValue(key: string): string | null {
    return this.dynamicFilters()[key] ?? null;
  }

  private parseDynamicFilters(raw: string): Record<string, string> {
    if (!raw) return {};
    try {
      const parsed = JSON.parse(raw) as Record<string, unknown>;
      if (parsed == null || typeof parsed !== 'object' || Array.isArray(parsed)) return {};
      return Object.entries(parsed).reduce<Record<string, string>>((acc, [key, value]) => {
        if (typeof value === 'string' && value.trim() !== '') acc[key] = value;
        return acc;
      }, {});
    } catch {
      return {};
    }
  }

  getMediaFileUrl(mediaId: string): string {
    return this.catalog.getMediaFileUrl(mediaId);
  }
}
