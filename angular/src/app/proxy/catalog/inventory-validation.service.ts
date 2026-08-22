import { RestService, Rest } from '@abp/ng.core';
import { Injectable, inject } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class InventoryValidationService {
  private restService = inject(RestService);
  apiName = 'Default';
  

  validateVariantAvailability = (productVariantId: string, quantity: number, config?: Partial<Rest.Config>) =>
    this.restService.request<any, void>({
      method: 'POST',
      url: `/api/app/inventory-validation/validate-variant-availability/${productVariantId}`,
      params: { quantity },
    },
    { apiName: this.apiName,...config });
}