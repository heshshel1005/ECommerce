import { authGuard, permissionGuard, eLayoutType } from '@abp/ng.core';
import { Routes } from '@angular/router';
import { redirectAdminToDashboardGuard, redirectNonAdminToCustomerGuard } from './storefront/redirect-admin.guard';

/** Permission required for admin-only routes (identity, tenants, settings). ABP permissionGuard reads this from route.data.requiredPolicy. */
export const ADMIN_PERMISSION = 'ECommerce.Administration';

/** Host-only: organization (tenant) signup queue. */
export const TENANT_SIGNUP_MANAGE_PERMISSION = 'ECommerce.TenantSignup.Manage';

export const APP_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./storefront/storefront-layout.component').then(c => c.StorefrontLayoutComponent),
    data: { layout: eLayoutType.empty },
    children: [
      {
        path: '',
        pathMatch: 'full',
        canActivate: [redirectAdminToDashboardGuard],
        loadComponent: () =>
          import('./storefront/storefront-home.component').then(c => c.StorefrontHomeComponent),
      },
      {
        path: 'catalog',
        loadComponent: () =>
          import('./storefront/catalog/product-list.component').then(c => c.StorefrontProductListComponent),
      },
      {
        path: 'catalog/product/:id',
        loadComponent: () =>
          import('./storefront/catalog/product-detail.component').then(c => c.StorefrontProductDetailComponent),
      },
      {
        path: 'catalog/compare',
        loadComponent: () =>
          import('./storefront/catalog/product-compare.component').then(c => c.StorefrontProductCompareComponent),
      },
      {
        path: 'cart',
        loadComponent: () =>
          import('./storefront/cart-placeholder.component').then(c => c.CartPlaceholderComponent),
      },
      {
        path: 'registry/:slug',
        loadComponent: () =>
          import('./storefront/gift-registry/registry-view.component').then(c => c.RegistryViewComponent),
      },
      {
        path: 'checkout',
        loadComponent: () =>
          import('./storefront/checkout/checkout.component').then(c => c.CheckoutComponent),
      },
      {
        path: 'checkout/payment',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./storefront/payment/payment.component').then(c => c.PaymentComponent),
      },
      {
        path: 'checkout/payment/success',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./storefront/payment/payment-success.component').then(c => c.PaymentSuccessComponent),
      },
      {
        path: 'my-account',
        canActivate: [authGuard],
        loadComponent: () =>
          import('./storefront/customer-dashboard/customer-dashboard-layout.component').then(
            c => c.CustomerDashboardLayoutComponent,
          ),
        children: [
          {
            path: '',
            pathMatch: 'full',
            redirectTo: 'overview',
          },
          {
            path: 'overview',
            pathMatch: 'full',
            loadComponent: () =>
              import('./storefront/customer-dashboard/customer-dashboard-overview.component').then(
                c => c.CustomerDashboardOverviewComponent,
              ),
          },
          {
            path: 'orders',
            loadComponent: () =>
              import('./storefront/customer-dashboard/customer-order-history.component').then(
                c => c.CustomerOrderHistoryComponent,
              ),
          },
          {
            path: 'orders/:id',
            loadComponent: () =>
              import('./storefront/customer-dashboard/customer-order-detail.component').then(
                c => c.CustomerOrderDetailComponent,
              ),
          },
          {
            path: 'profile',
            loadComponent: () =>
              import('./storefront/customer-dashboard/customer-profile.component').then(
                c => c.CustomerProfileComponent,
              ),
          },
          {
            path: 'wishlist',
            loadComponent: () =>
              import('./storefront/customer-dashboard/customer-wishlist.component').then(
                c => c.CustomerWishlistComponent,
              ),
          },
        ],
      },
      {
        path: 'newsletter/unsubscribe',
        loadComponent: () =>
          import('./storefront/newsletter-unsubscribe.component').then(c => c.NewsletterUnsubscribeComponent),
      },
      {
        path: 'organization/register',
        loadComponent: () =>
          import('./storefront/organization-signup.component').then(c => c.OrganizationSignupComponent),
      },
      {
        path: 'page/:slug',
        loadComponent: () =>
          import('./cms/cms-page.component').then(c => c.CmsPageComponent),
      },
      {
        path: 'blog',
        loadComponent: () =>
          import('./cms/blog-list.component').then(c => c.BlogListComponent),
      },
    ],
  },
  {
    path: 'admin',
    canActivate: [authGuard, redirectNonAdminToCustomerGuard, permissionGuard],
    data: { requiredPolicy: ADMIN_PERMISSION },
    children: [
      {
        path: '',
        pathMatch: 'full',
        redirectTo: 'dashboard',
      },
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./admin/admin-dashboard.component').then((c) => c.AdminDashboardComponent),
      },
      {
        path: 'home',
        loadComponent: () =>
          import('./admin/admin-home.component').then((c) => c.AdminHomeComponent),
      },
      {
        path: 'catalog',
        children: [
          {
            path: 'categories',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/categories/category-list.component').then(
                (c) => c.CategoryListComponent,
              ),
          },
          {
            path: 'categories/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/categories/category-edit.component').then(
                (c) => c.CategoryEditComponent,
              ),
          },
          {
            path: 'categories/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/categories/category-edit.component').then(
                (c) => c.CategoryEditComponent,
              ),
          },
          {
            path: 'brands',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.Brands' },
            loadComponent: () =>
              import('./admin/catalog/brands/brand-list.component').then(
                (c) => c.BrandListComponent,
              ),
          },
          {
            path: 'brands/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.Brands' },
            loadComponent: () =>
              import('./admin/catalog/brands/brand-edit.component').then(
                (c) => c.BrandEditComponent,
              ),
          },
          {
            path: 'brands/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.Brands' },
            loadComponent: () =>
              import('./admin/catalog/brands/brand-edit.component').then(
                (c) => c.BrandEditComponent,
              ),
          },
          {
            path: 'models',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.BrandModels' },
            loadComponent: () =>
              import('./admin/catalog/brand-models/brand-model-list.component').then(
                (c) => c.BrandModelListComponent,
              ),
          },
          {
            path: 'models/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.BrandModels' },
            loadComponent: () =>
              import('./admin/catalog/brand-models/brand-model-edit.component').then(
                (c) => c.BrandModelEditComponent,
              ),
          },
          {
            path: 'models/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog.BrandModels' },
            loadComponent: () =>
              import('./admin/catalog/brand-models/brand-model-edit.component').then(
                (c) => c.BrandModelEditComponent,
              ),
          },
          {
            path: 'products',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/products/product-list.component').then(
                (c) => c.ProductListComponent,
              ),
          },
          {
            path: 'products/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/products/product-edit.component').then(
                (c) => c.ProductEditComponent,
              ),
          },
          {
            path: 'products/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/products/product-edit.component').then(
                (c) => c.ProductEditComponent,
              ),
          },
          {
            path: 'product-types',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/product-types/product-type-list.component').then(
                (c) => c.ProductTypeListComponent,
              ),
          },
          {
            path: 'product-types/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/product-types/product-type-edit.component').then(
                (c) => c.ProductTypeEditComponent,
              ),
          },
          {
            path: 'product-types/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/product-types/product-type-edit.component').then(
                (c) => c.ProductTypeEditComponent,
              ),
          },
          {
            path: 'attribute-definitions',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/attribute-definitions/attribute-definition-list.component').then(
                (c) => c.AttributeDefinitionListComponent,
              ),
          },
          {
            path: 'attribute-definitions/create',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/attribute-definitions/attribute-definition-edit.component').then(
                (c) => c.AttributeDefinitionEditComponent,
              ),
          },
          {
            path: 'attribute-definitions/edit/:id',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/metadata/attribute-definitions/attribute-definition-edit.component').then(
                (c) => c.AttributeDefinitionEditComponent,
              ),
          },
          {
            path: 'reviews',
            canActivate: [permissionGuard],
            data: { requiredPolicy: 'ECommerce.Catalog' },
            loadComponent: () =>
              import('./admin/catalog/reviews/review-list.component').then(
                (c) => c.ReviewListComponent,
              ),
          },
        ],
      },
      {
        path: 'orders',
        children: [
          {
            path: '',
            pathMatch: 'full',
            loadComponent: () =>
              import('./admin/orders/order-list.component').then((c) => c.OrderListComponent),
          },
          {
            path: ':id',
            loadComponent: () =>
              import('./admin/orders/order-detail.component').then((c) => c.OrderDetailComponent),
          },
        ],
      },
      {
        path: 'coupons',
        loadComponent: () =>
          import('./admin/coupons/coupon-list.component').then((c) => c.CouponListComponent),
      },
      {
        path: 'newsletter',
        loadComponent: () =>
          import('./admin/newsletter/newsletter-admin.component').then((c) => c.NewsletterAdminComponent),
      },
      {
        path: 'analytics',
        loadComponent: () =>
          import('./admin/analytics/admin-analytics.component').then((c) => c.AdminAnalyticsComponent),
      },
      {
        path: 'organization-signups',
        canActivate: [permissionGuard],
        data: { requiredPolicy: TENANT_SIGNUP_MANAGE_PERMISSION },
        children: [
          {
            path: '',
            pathMatch: 'full',
            loadComponent: () =>
              import('./admin/organization-signup/organization-signup-queue-list.component').then(
                (c) => c.OrganizationSignupQueueListComponent,
              ),
          },
          {
            path: ':id',
            loadComponent: () =>
              import('./admin/organization-signup/organization-signup-queue-detail.component').then(
                (c) => c.OrganizationSignupQueueDetailComponent,
              ),
          },
        ],
      },
      {
        path: 'cms',
        children: [
          {
            path: 'pages',
            pathMatch: 'full',
            loadComponent: () =>
              import('./admin/cms/page-list.component').then((c) => c.CmsPageListComponent),
          },
          {
            path: 'blogs',
            pathMatch: 'full',
            loadComponent: () =>
              import('./admin/cms/blog-list.component').then((c) => c.CmsBlogListComponent),
          },
          {
            path: 'blogs/:blogId/posts',
            loadComponent: () =>
              import('./admin/cms/blog-post-list.component').then((c) => c.CmsBlogPostListComponent),
          },
        ],
      },
    ],
  },
  {
    path: 'account',
    children: [
      {
        path: 'confirm-email',
        loadComponent: () =>
          import('./account/confirm-email.component').then((c) => c.ConfirmEmailComponent),
      },
      {
        path: 'subscribe',
        loadComponent: () =>
          import('./account/subscription.component').then((c) => c.SubscriptionComponent),
      },
      {
        path: 'register',
        loadComponent: () =>
          import('./account/subscription.component').then((c) => c.SubscriptionComponent),
      },
      {
        path: '',
        loadChildren: () => import('@abp/ng.account').then((c) => c.createRoutes()),
      },
    ],
  },
  {
    path: 'identity',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: ADMIN_PERMISSION },
    loadChildren: () => import('@abp/ng.identity').then(c => c.createRoutes()),
  },
  {
    path: 'tenant-management',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: ADMIN_PERMISSION },
    loadChildren: () => import('@abp/ng.tenant-management').then(c => c.createRoutes()),
  },
  {
    path: 'setting-management',
    canActivate: [authGuard, permissionGuard],
    data: { requiredPolicy: ADMIN_PERMISSION },
    loadChildren: () => import('@abp/ng.setting-management').then(c => c.createRoutes()),
  },
];
