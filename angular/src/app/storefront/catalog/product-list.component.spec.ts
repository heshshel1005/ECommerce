import '@angular/compiler';
import { ActivatedRoute, Router } from '@angular/router';
import { SessionStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { BehaviorSubject, of } from 'rxjs';
import { beforeEach, describe, expect, it, vi } from 'vitest';
import { TestBed } from '@angular/core/testing';
import { StorefrontProductListComponent } from './product-list.component';
import { PublicCatalogService } from '../public-catalog.service';
import { BreadcrumbService } from '../../shared/breadcrumbs/breadcrumb.service';

describe('StorefrontProductListComponent', () => {
  const queryParams$ = new BehaviorSubject<Record<string, string>>({});
  const language$ = new BehaviorSubject('en');
  const getCategoryTree = vi.fn();
  const getFilterOptions = vi.fn();
  const getProductList = vi.fn();
  const navigate = vi.fn();

  let component: StorefrontProductListComponent;

  beforeEach(() => {
    queryParams$.next({});
    language$.next('en');
    navigate.mockReset();
    getCategoryTree.mockReturnValue(
      of([{ id: 'cat-1', name: 'Category', displayOrder: 1, children: [] }])
    );
    getFilterOptions.mockReturnValue(
      of({ sizes: [], colors: [], brands: [], models: [] })
    );
    getProductList
      .mockReturnValueOnce(
        of({
          items: [
            {
              id: 'p1',
              productNumber: 'SKU-1',
              name: 'Phone',
              isInStock: true,
              brandId: 'b1',
            },
          ],
          totalCount: 1,
        })
      )
      .mockReturnValueOnce(
        of({
          items: [
            {
              id: 'p1',
              productNumber: 'SKU-1',
              name: 'Telephone',
              isInStock: true,
              brandId: 'b1',
            },
          ],
          totalCount: 1,
        })
      );

    TestBed.configureTestingModule({
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: queryParams$.asObservable(),
          },
        },
        {
          provide: Router,
          useValue: {
            navigate,
          },
        },
        {
          provide: PublicCatalogService,
          useValue: {
            getCategoryTree,
            getFilterOptions,
            getProductList,
            getMediaFileUrl: vi.fn().mockReturnValue('/media/mock'),
          },
        },
        {
          provide: ToasterService,
          useValue: {
            error: vi.fn(),
          },
        },
        {
          provide: BreadcrumbService,
          useValue: {
            setItems: vi.fn(),
            clear: vi.fn(),
          },
        },
        {
          provide: SessionStateService,
          useValue: {
            getLanguage$: () => language$.asObservable(),
          },
        },
      ],
    });

    component = TestBed.runInInjectionContext(
      () => new StorefrontProductListComponent()
    );
  });

  it('reloads catalog data when language changes and keeps localized names', () => {
    component.ngOnInit();

    // Initial BehaviorSubject emission triggers first load.
    expect(getProductList).toHaveBeenCalledTimes(1);
    expect(component.items()[0]?.name).toBe('Phone');

    // Second language emission passes skip(1) and triggers reload.
    language$.next('fr');

    expect(getCategoryTree).toHaveBeenCalledTimes(2);
    expect(getFilterOptions).toHaveBeenCalledTimes(2);
    expect(getProductList).toHaveBeenCalledTimes(2);
    expect(component.items()[0]?.name).toBe('Telephone');
  });

  it('applies AUTO_PART dynamic filters from query params and forwards filter payload to catalog API', () => {
    getFilterOptions.mockReturnValueOnce(
      of({
        attributes: [
          { key: 'condition', displayName: 'Etat', values: ['NEW', 'USED'] },
          { key: 'fitment_type', displayName: 'Type de montage', values: ['DIRECT_FIT', 'UNIVERSAL'] },
        ],
        brands: [],
        models: [],
      })
    );
    getProductList.mockClear();
    getProductList.mockReturnValue(
      of({
        items: [
          {
            id: 'p-auto-1',
            productNumber: 'AP-100',
            name: 'Brake Pad',
            isInStock: true,
            brandId: 'b1',
          },
        ],
        totalCount: 1,
      })
    );
    queryParams$.next({
      filters: JSON.stringify({ condition: 'NEW', fitment_type: 'DIRECT_FIT' }),
    });

    component.ngOnInit();

    expect(getProductList).toHaveBeenCalledWith(
      expect.objectContaining({
        dynamicFiltersJson: JSON.stringify({
          condition: 'NEW',
          fitment_type: 'DIRECT_FIT',
        }),
      })
    );
    expect(component.getDynamicAttributeValue('condition')).toBe('NEW');
    expect(component.filterOptions().attributes.map((x) => x.key)).toEqual([
      'condition',
      'fitment_type',
    ]);
    expect(component.filterOptions().attributes.map((x) => x.displayName)).toEqual([
      'Etat',
      'Type de montage',
    ]);

    component.onDynamicAttributeChange('condition', 'USED');

    expect(navigate).toHaveBeenCalledWith(
      [],
      expect.objectContaining({
        queryParams: expect.objectContaining({
          filters: JSON.stringify({
            condition: 'USED',
            fitment_type: 'DIRECT_FIT',
          }),
        }),
      })
    );
  });
});
