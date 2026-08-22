import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

export interface LoyaltySettingsDto {
  pointsPerCurrencyUnit: string;
}

export interface UpdateLoyaltySettingsDto {
  pointsPerCurrencyUnit: string;
}

@Injectable({ providedIn: 'root' })
export class LoyaltySettingsService {
  private readonly rest = inject(RestService);

  get(): Observable<LoyaltySettingsDto> {
    return this.rest.request<void, LoyaltySettingsDto>({
      method: 'GET',
      url: '/api/app/loyalty-settings',
    });
  }

  update(body: UpdateLoyaltySettingsDto): Observable<void> {
    return this.rest.request<UpdateLoyaltySettingsDto, void>({
      method: 'POST',
      url: '/api/app/loyalty-settings',
      body,
    });
  }
}
