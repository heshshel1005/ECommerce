import { Component, inject, input, computed } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';
import type { BreadcrumbItem } from './breadcrumb-item.interface';
import { BreadcrumbService } from './breadcrumb.service';

/**
 * Reusable breadcrumbs for the storefront. Renders from BreadcrumbService by default,
 * or from the optional [items] input when provided.
 */
@Component({
  selector: 'app-breadcrumbs',
  templateUrl: './breadcrumbs.component.html',
  styleUrls: ['./breadcrumbs.component.scss'],
  imports: [RouterLink, LocalizationPipe],
})
export class BreadcrumbsComponent {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly serviceItems = toSignal(this.breadcrumbService.getItems(), { initialValue: [] as BreadcrumbItem[] });

  /** When set, use these items instead of the service. */
  itemsInput = input<BreadcrumbItem[] | null>(null);

  /** Items to display: from input or from service. */
  items = computed(() => {
    const fromInput = this.itemsInput();
    if (fromInput && fromInput.length > 0) return fromInput;
    return this.serviceItems() ?? [];
  });

  readonly homeRoute = ['/'];
}
