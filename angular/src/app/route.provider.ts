import { RoutesService, eLayoutType } from '@abp/ng.core';
import { inject, provideAppInitializer } from '@angular/core';

/** Permission required for admin menu items and routes. */
const ADMIN_PERMISSION = 'ECommerce.Administration';

/** Catalog permissions for tenant-only catalog admin pages. */
const CATALOG_DEFAULT_PERMISSION = 'ECommerce.Catalog';
const CATALOG_BRANDS_PERMISSION = 'ECommerce.Catalog.Brands';
const CATALOG_BRAND_MODELS_PERMISSION = 'ECommerce.Catalog.BrandModels';

const ADMINISTRATION_PARENT = 'AbpUiNavigation::Menu:Administration';

export const APP_ROUTE_PROVIDER = [
  provideAppInitializer(() => {
    configureRoutes();
  }),
];

function configureRoutes() {
  const routes = inject(RoutesService);
  routes.add([
    {
      path: '/',
      name: 'ECommerce::StorefrontHome',
      iconClass: 'fas fa-home',
      order: 1,
      layout: eLayoutType.empty,
    },
    {
      path: '/catalog',
      name: 'ECommerce::Catalog',
      iconClass: 'fas fa-th-list',
      order: 2,
      layout: eLayoutType.empty,
    },
    {
      path: '/cart',
      name: 'ECommerce::Cart',
      iconClass: 'fas fa-shopping-cart',
      order: 3,
      layout: eLayoutType.empty,
    },
    {
      path: '/my-account',
      name: 'ECommerce::MyAccount',
      iconClass: 'fas fa-user',
      order: 4,
      layout: eLayoutType.empty,
    },
    {
      path: '/organization/register',
      name: 'ECommerce::OrganizationSignupPageTitle',
      iconClass: 'fas fa-building',
      order: 3.5,
      layout: eLayoutType.empty,
    },
    {
      path: '/admin/home',
      name: 'ECommerce::AdminHome',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-home',
      order: 0,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/dashboard',
      name: 'ECommerce::Dashboard',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-tachometer-alt',
      order: 0.5,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/categories',
      name: 'ECommerce::Categories',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_DEFAULT_PERMISSION,
      iconClass: 'fas fa-sitemap',
      order: 1,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/brands',
      name: 'ECommerce::Brands',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_BRANDS_PERMISSION,
      iconClass: 'fas fa-industry',
      order: 1.1,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/models',
      name: 'ECommerce::Models',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_BRAND_MODELS_PERMISSION,
      iconClass: 'fas fa-car-side',
      order: 1.2,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/products',
      name: 'ECommerce::Products',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_DEFAULT_PERMISSION,
      iconClass: 'fas fa-box',
      order: 2,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/product-types',
      name: 'ECommerce::ProductTypes',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_DEFAULT_PERMISSION,
      iconClass: 'fas fa-tags',
      order: 2.1,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/attribute-definitions',
      name: 'ECommerce::AttributeDefinitions',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_DEFAULT_PERMISSION,
      iconClass: 'fas fa-sliders-h',
      order: 2.2,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/catalog/reviews',
      name: 'ECommerce::ProductReviews',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: CATALOG_DEFAULT_PERMISSION,
      iconClass: 'fas fa-star',
      order: 3,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/orders',
      name: 'ECommerce::AdminOrders',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-shopping-bag',
      order: 4,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/coupons',
      name: 'ECommerce::Coupons',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-tag',
      order: 5,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/newsletter',
      name: 'ECommerce::Newsletter',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-envelope',
      order: 6,
      layout: eLayoutType.application,
    },
    {
      path: '/identity/users',
      name: 'AbpIdentity::Users',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-users',
      order: 6.5,
      layout: eLayoutType.application,
    },
    {
      path: '/identity/roles',
      name: 'AbpIdentity::Roles',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-user-tag',
      order: 6.6,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/cms/pages',
      name: 'ECommerce::CmsPages',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-file-alt',
      order: 7,
      layout: eLayoutType.application,
    },
    {
      path: '/admin/cms/blogs',
      name: 'ECommerce::CmsBlogs',
      parentName: ADMINISTRATION_PARENT,
      requiredPolicy: ADMIN_PERMISSION,
      iconClass: 'fas fa-blog',
      order: 8,
      layout: eLayoutType.application,
    },
  ]);
}
