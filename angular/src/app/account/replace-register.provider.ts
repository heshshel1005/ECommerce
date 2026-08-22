import { inject, provideAppInitializer } from '@angular/core';
import { ReplaceableComponentsService } from '@abp/ng.core';
import { SubscriptionComponent } from './subscription.component';

/** Replace ABP's default register form (user/email/password only) with our full customer subscription form (contact + addresses). */
const ACCOUNT_REGISTER_KEY = 'Account.RegisterComponent';

export const REPLACE_ACCOUNT_REGISTER_PROVIDER = [
  provideAppInitializer(() => {
    const replaceable = inject(ReplaceableComponentsService);
    replaceable.add({
      key: ACCOUNT_REGISTER_KEY,
      component: SubscriptionComponent,
    });
  }),
];
