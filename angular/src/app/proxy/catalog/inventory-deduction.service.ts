import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InventoryDeductionService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  deductForOrderLines = (lines: Record<string, number>, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/inventory-deduction/deduct-for-order-lines',
      body: lines,
    },
    { apiName: this.apiName,...config });
  

  restoreForOrderLines = (lines: Record<string, number>, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: '/api/app/inventory-deduction/restore-for-order-lines',
      body: lines,
    },
    { apiName: this.apiName,...config });
}