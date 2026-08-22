import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { map, take } from 'rxjs';
import { AuthService, PermissionService } from '@abp/ng.core';

/** Permission required for admin; must match app.routes ADMIN_PERMISSION. */
const ADMIN_PERMISSION = 'ECommerce.Administration';

/**
 * When navigating to the storefront home (/), redirect users who have the
 * Administration permission to the admin dashboard so they see the admin UI.
 * Uses observable so we wait for permissions (loaded after login) before deciding.
 */
export const redirectAdminToDashboardGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const permission = inject(PermissionService);
  const router = inject(Router);

  if (!auth.isAuthenticated) return true;

  return permission.getGrantedPolicy$(ADMIN_PERMISSION).pipe(
    take(1),
    map(hasAdmin => (hasAdmin ? router.createUrlTree(['/admin/dashboard']) : true)),
  );
};

/**
 * When navigating to any /admin route, redirect users who do NOT have the
 * Administration permission to the storefront home (/) so Customer role users
 * see the same view as guest users (storefront with Catalog, Cart, etc.).
 */
export const redirectNonAdminToCustomerGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const permission = inject(PermissionService);
  const router = inject(Router);

  if (!auth.isAuthenticated) return true;

  return permission.getGrantedPolicy$(ADMIN_PERMISSION).pipe(
    take(1),
    map(hasAdmin => (hasAdmin ? true : router.createUrlTree(['/']))),
  );
};
