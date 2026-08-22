import { Component, inject, OnInit, OnDestroy, signal } from '@angular/core';
import { DecimalPipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { BreadcrumbService } from '../shared/breadcrumbs/breadcrumb.service';
import { CartService, CartDto, CartItemDto } from './cart.service';

@Component({
  selector: 'app-cart-placeholder',
  standalone: true,
  imports: [DecimalPipe, LocalizationPipe, RouterLink],
  templateUrl: './cart-placeholder.component.html',
  styles: [
    `
      .quantity-display {
        min-width: 2.5rem;
        display: inline-block;
        text-align: center;
        font-weight: 500;
      }
    `,
  ],
})
export class CartPlaceholderComponent implements OnInit, OnDestroy {
  private readonly breadcrumbService = inject(BreadcrumbService);
  private readonly cartService = inject(CartService);
  private readonly toaster = inject(ToasterService);

  cart = signal<CartDto | null>(null);
  /** Cart items for template (empty array when no cart or no items). */
  cartItems = signal<CartItemDto[]>([]);
  loading = signal(true);
  updatingId = signal<string | null>(null);
  removingId = signal<string | null>(null);

  ngOnInit() {
    this.breadcrumbService.setItems([{ label: 'ECommerce::Cart' }]);
    this.loadCart();
  }

  ngOnDestroy() {
    this.breadcrumbService.clear();
  }

  loadCart(): void {
    this.loading.set(true);
    this.cartService.getCart().subscribe({
      next: (c) => {
        this.cart.set(c);
        this.cartItems.set(c?.items ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.cart.set(null);
        this.cartItems.set([]);
        this.loading.set(false);
      },
    });
  }

  lineTotal(item: CartItemDto): number {
    const price = item.unitPrice ?? 0;
    return price * item.quantity;
  }

  subtotal(): number {
    const c = this.cart();
    if (!c?.items?.length) return 0;
    return c.items.reduce((sum, it) => sum + this.lineTotal(it), 0);
  }

  updateQuantity(item: CartItemDto, newQty: number): void {
    if (newQty < 1) return;
    const max = item.availableStock ?? newQty;
    const qty = Math.min(newQty, max > 0 ? max : newQty);
    this.updatingId.set(item.id);
    this.cartService.updateItem(item.id, qty).subscribe({
      next: (updated) => {
        this.cart.set(updated);
        this.cartItems.set(updated?.items ?? []);
        this.updatingId.set(null);
      },
      error: (err) => {
        this.updatingId.set(null);
        const msg = err?.error?.error?.message ?? 'ECommerce::ErrorLoadingProducts';
        this.toaster.error(msg, 'ECommerce::Cart');
      },
    });
  }

  removeItem(item: CartItemDto): void {
    this.removingId.set(item.id);
    this.cartService.removeItem(item.id).subscribe({
      next: (updated) => {
        this.cart.set(updated);
        this.cartItems.set(updated?.items ?? []);
        this.removingId.set(null);
      },
      error: () => {
        this.removingId.set(null);
        this.toaster.error('ECommerce::ErrorLoadingProducts', 'ECommerce::Cart');
      },
    });
  }
}
