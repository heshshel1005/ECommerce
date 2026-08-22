import { Injectable, inject, signal } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { AuthService } from '@abp/ng.core';
import { Observable } from 'rxjs';
import { map, tap } from 'rxjs/operators';

export const GUEST_CART_ID_KEY = 'guestCartId';

export interface CartItemDto {
  id: string;
  cartId: string;
  productVariantId: string;
  productId: string;
  productName: string;
  sku: string;
  unitPrice?: number | null;
  quantity: number;
  availableStock?: number | null;
}

export interface CartDto {
  id: string;
  isAuthenticated: boolean;
  items: CartItemDto[];
  itemCount: number;
}

@Injectable({ providedIn: 'root' })
export class CartService {
  private readonly rest = inject(RestService);
  private readonly authService = inject(AuthService);

  /** Current cart item count (total quantity). Updated whenever cart is fetched or modified. */
  readonly cartItemCount = signal(0);

  /** For guests: get or create guest cart id (stored in sessionStorage). */
  getGuestCartId(): string | null {
    if (this.authService.isAuthenticated) return null;
    try {
      let id = sessionStorage.getItem(GUEST_CART_ID_KEY);
      if (!id) {
        id = this.generateGuid();
        sessionStorage.setItem(GUEST_CART_ID_KEY, id);
      }
      return id;
    } catch {
      return null;
    }
  }

  setGuestCartId(id: string | null): void {
    try {
      if (id) sessionStorage.setItem(GUEST_CART_ID_KEY, id);
      else sessionStorage.removeItem(GUEST_CART_ID_KEY);
    } catch {}
  }

  getCart(): Observable<CartDto> {
    const params: Record<string, string> = {};
    const gid = this.getGuestCartId();
    if (gid) params.guestCartId = gid;
    return this.rest.request<void, CartDto>({
      method: 'GET',
      url: '/api/app/cart',
      params: Object.keys(params).length ? params : undefined,
    }).pipe(
      map((res) => this.normalizeCart(res)),
      tap((c) => this.cartItemCount.set(c.itemCount))
    );
  }

  addItem(productVariantId: string, quantity: number): Observable<CartDto> {
    const params: Record<string, string> = {};
    const gid = this.getGuestCartId();
    if (gid) params.guestCartId = gid;
    return this.rest.request<{ productVariantId: string; quantity: number }, CartDto>({
      method: 'POST',
      url: '/api/app/cart/items',
      body: { productVariantId, quantity },
      params: Object.keys(params).length ? params : undefined,
    }).pipe(
      map((res) => this.normalizeCart(res)),
      tap((c) => this.cartItemCount.set(c.itemCount))
    );
  }

  updateItem(cartItemId: string, quantity: number): Observable<CartDto> {
    const params: Record<string, string> = {};
    const gid = this.getGuestCartId();
    if (gid) params.guestCartId = gid;
    return this.rest.request<{ quantity: number }, CartDto>({
      method: 'PUT',
      url: `/api/app/cart/items/${cartItemId}`,
      body: { quantity },
      params: Object.keys(params).length ? params : undefined,
    }).pipe(
      map((res) => this.normalizeCart(res)),
      tap((c) => this.cartItemCount.set(c.itemCount))
    );
  }

  removeItem(cartItemId: string): Observable<CartDto> {
    const params: Record<string, string> = {};
    const gid = this.getGuestCartId();
    if (gid) params.guestCartId = gid;
    return this.rest.request<void, CartDto>({
      method: 'DELETE',
      url: `/api/app/cart/items/${cartItemId}`,
      params: Object.keys(params).length ? params : undefined,
    }).pipe(
      map((res) => this.normalizeCart(res)),
      tap((c) => this.cartItemCount.set(c.itemCount))
    );
  }

  private normalizeCart(res: unknown): CartDto {
    const o = (res != null && typeof res === 'object' ? res : {}) as Record<string, unknown>;
    const items = (o.items ?? o.Items ?? []) as unknown[];
    return {
      id: (o.id ?? o.Id) as string,
      isAuthenticated: (o.isAuthenticated ?? o.IsAuthenticated) as boolean ?? false,
      itemCount: (o.itemCount ?? o.ItemCount) as number ?? 0,
      items: items.map((it) => {
        const i = (it != null && typeof it === 'object' ? it : {}) as Record<string, unknown>;
        return {
          id: (i.id ?? i.Id) as string,
          cartId: (i.cartId ?? i.CartId) as string,
          productVariantId: (i.productVariantId ?? i.ProductVariantId) as string,
          productId: (i.productId ?? i.ProductId) as string,
          productName: (i.productName ?? i.ProductName) as string ?? '',
          sku: (i.sku ?? i.Sku) as string ?? '',
          unitPrice: (i.unitPrice ?? i.UnitPrice) as number | null,
          quantity: (i.quantity ?? i.Quantity) as number ?? 0,
          availableStock: (i.availableStock ?? i.AvailableStock) as number | null,
        };
      }),
    };
  }

  private generateGuid(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (c) => {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });
  }
}
