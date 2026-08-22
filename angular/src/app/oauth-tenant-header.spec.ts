import { TestBed } from '@angular/core/testing';
import {
  HTTP_INTERCEPTORS,
  HttpClient,
  provideHttpClient,
  withInterceptorsFromDi,
} from '@angular/common/http';
import {
  HttpTestingController,
  provideHttpClientTesting,
} from '@angular/common/http/testing';
import { OAuthApiInterceptor } from '@abp/ng.oauth';
import { ApiInterceptor, HttpWaitService, SessionStateService, TENANT_KEY } from '@abp/ng.core';
import { OAuthService } from 'angular-oauth2-oidc';
import { afterEach, beforeEach, describe, expect, it } from 'vitest';

describe('OAuthApiInterceptor', () => {
  let http: HttpClient;
  let httpMock: HttpTestingController;

  function configureTenantSession(
    tenant: { id?: string; name?: string; isAvailable?: boolean } | null,
  ) {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptorsFromDi()),
        provideHttpClientTesting(),
        { provide: ApiInterceptor, useClass: OAuthApiInterceptor },
        { provide: HTTP_INTERCEPTORS, useExisting: ApiInterceptor, multi: true },
        { provide: OAuthService, useValue: { getAccessToken: () => null } },
        {
          provide: SessionStateService,
          useValue: {
            getLanguage: () => 'en',
            getTenant: () => tenant,
          },
        },
        {
          provide: HttpWaitService,
          useValue: {
            addRequest: () => {},
            deleteRequest: () => {},
          },
        },
        { provide: TENANT_KEY, useValue: '__tenant' },
      ],
    });

    http = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  }

  beforeEach(() => {
    TestBed.resetTestingModule();
  });

  afterEach(() => {
    httpMock.verify();
    TestBed.resetTestingModule();
  });

  it('sends __tenant header with the current tenant id when set', () => {
    const tenantId = '11111111-1111-1111-1111-111111111111';
    configureTenantSession({ id: tenantId, name: 'Acme', isAvailable: true });

    http.get('/api/x').subscribe();

    const req = httpMock.expectOne('/api/x');
    expect(req.request.headers.get('__tenant')).toBe(tenantId);
    req.flush({});
  });

  it('does not send __tenant when tenant id is missing (host)', () => {
    configureTenantSession({ id: undefined, name: undefined, isAvailable: true });

    http.get('/api/x').subscribe();

    const req = httpMock.expectOne('/api/x');
    expect(req.request.headers.has('__tenant')).toBe(false);
    req.flush({});
  });
});
