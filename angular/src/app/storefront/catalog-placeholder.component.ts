import { Component, inject, OnInit, OnDestroy } from '@angular/core';
import { LocalizationPipe } from '@abp/ng.core';
import { BreadcrumbService } from '../shared/breadcrumbs/breadcrumb.service';

@Component({
  selector: 'app-catalog-placeholder',
  template: `
    <div class="card">
      <div class="card-body">
        <h2 class="card-title">{{ 'ECommerce::Catalog' | abpLocalization }}</h2>
        <p class="text-muted">{{ 'ECommerce::DashboardCatalogDescription' | abpLocalization }}</p>
        <p class="mb-0"><small>Catalog listing will be implemented in Plan 2.</small></p>
      </div>
    </div>
  `,
  imports: [LocalizationPipe],
})
export class CatalogPlaceholderComponent implements OnInit, OnDestroy {
  private readonly breadcrumbService = inject(BreadcrumbService);

  ngOnInit() {
    this.breadcrumbService.setItems([{ label: 'ECommerce::Catalog' }]);
  }

  ngOnDestroy() {
    this.breadcrumbService.clear();
  }
}
