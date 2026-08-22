import { Injectable, signal, computed } from '@angular/core';

const COMPARE_STORAGE_KEY = 'ecommerce_compare_product_ids';
const MAX_COMPARE = 4;

@Injectable({ providedIn: 'root' })
export class CompareService {
  private readonly ids = signal<string[]>(this.loadFromStorage());

  compareIds = computed(() => this.ids());

  isInCompare(productId: string): boolean {
    return this.ids().includes(productId);
  }

  add(productId: string): boolean {
    const current = this.ids();
    if (current.includes(productId)) return true;
    if (current.length >= MAX_COMPARE) return false;
    const next = [...current, productId];
    this.saveToStorage(next);
    this.ids.set(next);
    return true;
  }

  remove(productId: string): void {
    const next = this.ids().filter((id) => id !== productId);
    this.saveToStorage(next);
    this.ids.set(next);
  }

  getProductIds(): string[] {
    return [...this.ids()];
  }

  private loadFromStorage(): string[] {
    try {
      const raw = localStorage.getItem(COMPARE_STORAGE_KEY);
      if (!raw) return [];
      const arr = JSON.parse(raw) as unknown;
      return Array.isArray(arr) ? arr.filter((x): x is string => typeof x === 'string').slice(0, MAX_COMPARE) : [];
    } catch {
      return [];
    }
  }

  private saveToStorage(ids: string[]): void {
    try {
      localStorage.setItem(COMPARE_STORAGE_KEY, JSON.stringify(ids));
    } catch {
      // ignore
    }
  }
}
