import { ApplicationInfo, Environment } from '@abp/ng.core';

const baseUrl = 'http://localhost:4200';

const oAuthConfig = {
  issuer: 'https://localhost:44370/',
  redirectUri: baseUrl,
  clientId: 'ECommerce_App',
  responseType: 'code',
  scope: 'offline_access ECommerce',
  requireHttps: true,
};

export const environment = {
  production: false,
  application: {
    baseUrl,
    name: 'ECommerce',
    multiTenancy: {
      isEnabled: true,
    },
  },
  oAuthConfig,
  apis: {
    default: {
      url: 'https://localhost:44370',
      rootNamespace: 'ECommerce',
    },
    AbpAccountPublic: {
      url: oAuthConfig.issuer,
      rootNamespace: 'AbpAccountPublic',
    },
  },
} as Environment & {
  application: ApplicationInfo & { multiTenancy: { isEnabled: boolean } };
};
