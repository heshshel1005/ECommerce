import { Component, inject, ViewChild, ElementRef } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { LocalizationPipe, MultiTenancyService, SessionStateService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { SubscriptionService } from './subscription.service';
import type { AddressInputDto, CustomerRegisterDto } from './models/customer-register.dto';
import { firstValueFrom } from 'rxjs';

/** Email regex (same idea as Validators.email). */
const EMAIL_REGEX = /^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$/;

@Component({
  standalone: true,
  imports: [ReactiveFormsModule, RouterLink, LocalizationPipe],
  templateUrl: './subscription.component.html',
})
export class SubscriptionComponent {
  private readonly fb = inject(FormBuilder);
  private readonly subscription = inject(SubscriptionService);
  private readonly toaster = inject(ToasterService);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly multiTenancy = inject(MultiTenancyService);
  private readonly sessionState = inject(SessionStateService);

  @ViewChild('registrationForm') formRef?: ElementRef<HTMLFormElement>;
  @ViewChild('refUserName') refUserName?: ElementRef<HTMLInputElement>;
  @ViewChild('refEmail') refEmail?: ElementRef<HTMLInputElement>;
  @ViewChild('refPassword') refPassword?: ElementRef<HTMLInputElement>;
  @ViewChild('refDisplayName') refDisplayName?: ElementRef<HTMLInputElement>;
  @ViewChild('refPhone') refPhone?: ElementRef<HTMLInputElement>;
  @ViewChild('refShipLabel') refShipLabel?: ElementRef<HTMLInputElement>;
  @ViewChild('refShipStreet') refShipStreet?: ElementRef<HTMLInputElement>;

  inProgress = false;
  showValidationMessage = false;
  showPassword = false;

  form = this.fb.group({
    useSameBilling: [true],
    userName: ['', [Validators.required]],
    emailAddress: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    displayName: ['', [Validators.required]],
    phoneNumber: [''],
    shippingAddress: this.fb.group({
      label: ['Shipping', [Validators.required]],
      street: ['', [Validators.required]],
      city: [''],
      region: [''],
      postalCode: [''],
      country: [''],
    }),
    billingAddress: this.fb.group({
      label: ['Billing'],
      street: [''],
      city: [''],
      region: [''],
      postalCode: [''],
      country: [''],
    }),
  });

  /** Read value from an input ref (direct binding to our template inputs). */
  private val(ref: ElementRef<HTMLInputElement> | undefined): string {
    return ref?.nativeElement?.value?.trim() ?? '';
  }

  private ship(path: string) { return this.form.get('shippingAddress')?.get(path)?.value; }
  private bill(path: string) { return this.form.get('billingAddress')?.get(path)?.value; }

  /** Build and validate payload by reading directly from input refs, then form as fallback. */
  private getPayloadFromDomOrForm(): { valid: false } | { valid: true; input: CustomerRegisterDto } {
    const userName = this.val(this.refUserName) || (this.form.get('userName')?.value ?? '').toString().trim();
    const emailAddress = this.val(this.refEmail) || (this.form.get('emailAddress')?.value ?? '').toString().trim();
    const password = (this.val(this.refPassword) || (this.form.get('password')?.value ?? '').toString()).trim();
    const displayName = this.val(this.refDisplayName) || (this.form.get('displayName')?.value ?? '').toString().trim();
    const phoneNumber = this.val(this.refPhone) || (this.form.get('phoneNumber')?.value ?? '').toString().trim();
    const shipLabel = this.val(this.refShipLabel) || (this.ship('label') ?? '').toString().trim() || 'Shipping';
    const shipStreet = this.val(this.refShipStreet) || (this.ship('street') ?? '').toString().trim();
    const shipCity = this.ship('city');
    const shipRegion = this.ship('region');
    const shipPostal = this.ship('postalCode');
    const shipCountry = this.ship('country');

    if (!userName || !emailAddress || !password || password.length < 6 || !displayName || !shipStreet) {
      return { valid: false };
    }
    if (!EMAIL_REGEX.test(emailAddress)) {
      return { valid: false };
    }

    const useSameBilling = this.form.get('useSameBilling')?.value !== false;
    let billingAddress: AddressInputDto | undefined;
    if (!useSameBilling) {
      billingAddress = {
        label: (this.bill('label') ?? 'Billing').toString(),
        street: (this.bill('street') ?? '').toString(),
        city: this.bill('city'),
        region: this.bill('region'),
        postalCode: this.bill('postalCode'),
        country: this.bill('country'),
      };
    }

    const shipping: AddressInputDto = {
      label: (shipLabel ?? 'Shipping').toString(),
      street: shipStreet,
      city: shipCity ?? undefined,
      region: shipRegion ?? undefined,
      postalCode: shipPostal ?? undefined,
      country: shipCountry ?? undefined,
    };

    const input: CustomerRegisterDto = {
      userName,
      emailAddress,
      password,
      appName: 'Angular',
      displayName,
      phoneNumber: phoneNumber || undefined,
      shippingAddress: shipping,
      billingAddress,
    };
    return { valid: true, input };
  }

  async onSubmit(): Promise<void> {
    if (this.inProgress) return;
    this.showValidationMessage = false;
    this.form.markAllAsTouched();

    const result = this.getPayloadFromDomOrForm();
    if (!result.valid) {
      this.showValidationMessage = true;
      return;
    }

    const { input } = result;

    const tenantReady = await this.ensureTenantContextFromSelectorAsync();
    if (!tenantReady) {
      this.showValidationMessage = true;
      return;
    }

    this.inProgress = true;
    this.toaster.info('Registering…', '', { life: 2000 });
    this.subscription.subscribe(input).subscribe({
      next: () => {
        this.toaster.success('AbpAccount::RegisterSuccessMessage', '', { life: 7000 });
        const returnUrl = this.route.snapshot.queryParamMap.get('returnUrl');
        this.router.navigate(['/account/login'], {
          queryParams: {
            emailConfirmed: 'false',
            ...(returnUrl ? { returnUrl } : {}),
          },
        });
      },
      error: (err) => {
        const msg =
          err?.error?.error?.message
          ?? err?.error?.error_description
          ?? (typeof err?.message === 'string' ? err.message : null)
          ?? 'AbpAccount::DefaultErrorMessage';
        this.toaster.error(msg, 'Registration failed', { life: 7000 });
      },
      complete: () => {
        this.inProgress = false;
      },
    });
  }

  /**
   * Ensure ABP tenant session is aligned with tenant selector before /api/account/subscribe.
   * This guarantees the __tenant header is sent so registration happens in tenant scope.
   */
  private async ensureTenantContextFromSelectorAsync(): Promise<boolean> {
    const currentTenant = this.sessionState.getTenant();
    if (currentTenant?.id) {
      return true;
    }

    // ABP tenant box usually persists selector in query param as __tenant (fallback: tenant/tenancyName).
    const tenantName =
      (this.route.snapshot.queryParamMap.get('__tenant') ?? '').trim() ||
      (this.route.snapshot.queryParamMap.get('tenant') ?? '').trim() ||
      (this.route.snapshot.queryParamMap.get('tenancyName') ?? '').trim();

    if (!tenantName) {
      this.toaster.error('Select a tenant first (tenant selector) before registration.', 'Registration failed', {
        life: 7000,
      });
      return false;
    }

    try {
      const result = await firstValueFrom(this.multiTenancy.setTenantByName(tenantName));
      if (result?.success && !!result?.tenantId) {
        this.sessionState.setTenant({
          id: result.tenantId,
          name: result.name ?? tenantName,
          isAvailable: true,
        });
        return true;
      }

      this.toaster.error(`Tenant "${tenantName}" is not available.`, 'Registration failed', { life: 7000 });
      return false;
    } catch {
      this.toaster.error('Could not resolve tenant from tenant selector.', 'Registration failed', { life: 7000 });
      return false;
    }
  }
}
