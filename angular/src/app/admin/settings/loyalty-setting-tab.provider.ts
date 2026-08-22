import { inject, provideAppInitializer } from '@angular/core';
import { SettingTabsService } from '@abp/ng.setting-management/config';
import { LoyaltySettingGroupComponent } from './loyalty-setting-group.component';

export const LOYALTY_SETTING_TAB_PROVIDER = [
  provideAppInitializer(() => {
    const settingTabs = inject(SettingTabsService);
    settingTabs.add([
      {
        name: 'ECommerce::LoyaltyProgram',
        order: 99,
        requiredPolicy: 'ECommerce.Administration',
        component: LoyaltySettingGroupComponent,
      },
    ]);
  }),
];
