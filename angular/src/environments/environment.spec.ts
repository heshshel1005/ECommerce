import { describe, expect, it } from 'vitest';
import { environment as environmentDev } from './environment';
import { environment as environmentProd } from './environment.prod';

describe('Angular environment multi-tenancy', () => {
  it('enables multiTenancy in development environment', () => {
    expect(environmentDev.application.multiTenancy?.isEnabled).toBe(true);
  });

  it('enables multiTenancy in production environment', () => {
    expect(environmentProd.application.multiTenancy?.isEnabled).toBe(true);
  });
});
