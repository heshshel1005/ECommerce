import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import type { BreadcrumbItem } from './breadcrumb-item.interface';

@Injectable({ providedIn: 'root' })
export class BreadcrumbService {
  private readonly items$ = new BehaviorSubject<BreadcrumbItem[]>([]);

  /** Current breadcrumb items for the active route/page. */
  getItems(): Observable<BreadcrumbItem[]> {
    return this.items$.asObservable();
  }

  /** Set breadcrumb items (e.g. from a page component or route resolver). */
  setItems(items: BreadcrumbItem[]): void {
    this.items$.next(items ?? []);
  }

  /** Clear breadcrumbs. */
  clear(): void {
    this.items$.next([]);
  }
}
