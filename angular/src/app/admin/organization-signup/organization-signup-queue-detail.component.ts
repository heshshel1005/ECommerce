import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe, LocalizationService } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { OrganizationSignupHostService, OrganizationSignupRequestDto } from './organization-signup-host.service';

const PENDING = 0;
const MAX_REJECTION_REASON = 2000;

const SIGNUP_STATUS_NAMES = ['Pending', 'Approved', 'Rejected'] as const;

const BUSINESS_TYPE_NAMES = [
  'General',
  'AutoParts',
  'Clothing',
  'Electronics',
  'FoodAndBeverage',
  'HomeAndGarden',
  'HealthAndBeauty',
  'Sports',
  'Books',
  'Other',
] as const;

@Component({
  selector: 'app-organization-signup-queue-detail',
  standalone: true,
  imports: [DatePipe, RouterLink, ReactiveFormsModule, LocalizationPipe],
  templateUrl: './organization-signup-queue-detail.component.html',
})
export class OrganizationSignupQueueDetailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly hostService = inject(OrganizationSignupHostService);
  private readonly toaster = inject(ToasterService);
  private readonly fb = inject(FormBuilder);
  private readonly localization = inject(LocalizationService);

  request = signal<OrganizationSignupRequestDto | null>(null);
  loading = signal(true);
  approving = signal(false);
  rejecting = signal(false);
  showRejectForm = signal(false);

  rejectForm = this.fb.nonNullable.group({
    reason: ['', [Validators.required, Validators.maxLength(MAX_REJECTION_REASON)]],
  });

  isPending = computed(() => this.request()?.status === PENDING);

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) {
      this.loading.set(false);
      return;
    }
    this.hostService.get(id).subscribe({
      next: (r) => {
        this.request.set(r);
        this.loading.set(false);
      },
      error: () => {
        this.request.set(null);
        this.loading.set(false);
        this.toaster.error('ECommerce::OrganizationSignupQueueLoadError', 'Error');
      },
    });
  }

  statusLabel(status: number): string {
    const name = SIGNUP_STATUS_NAMES[status];
    return name ? `ECommerce::Enum:OrganizationSignupStatus.${name}` : 'ECommerce::NoData';
  }

  businessTypeLabel(type: number): string {
    const name = BUSINESS_TYPE_NAMES[type];
    return name ? `ECommerce::Enum:OrganizationBusinessType.${name}` : 'ECommerce::NoData';
  }

  approve(): void {
    const r = this.request();
    if (!r || !this.isPending() || this.approving()) return;
    const msg = this.localization.instant('ECommerce::OrganizationSignupQueueConfirmApprove');
    if (typeof window !== 'undefined' && !window.confirm(msg)) {
      return;
    }
    this.approving.set(true);
    this.hostService.approve(r.id).subscribe({
      next: () => {
        this.approving.set(false);
        this.toaster.success('ECommerce::OrganizationSignupQueueApproveSuccess');
        this.router.navigate(['/admin/organization-signups']);
      },
      error: (err) => {
        this.approving.set(false);
        const msg = err?.error?.error?.message ?? err?.message ?? 'ECommerce::OrganizationSignupQueueActionError';
        this.toaster.error(msg);
      },
    });
  }

  toggleRejectForm(): void {
    this.showRejectForm.update((v) => !v);
  }

  submitReject(): void {
    const r = this.request();
    if (!r || !this.isPending() || this.rejecting() || this.rejectForm.invalid) return;
    const reason = this.rejectForm.controls.reason.value.trim();
    if (!reason) return;
    this.rejecting.set(true);
    this.hostService.reject(r.id, { reason }).subscribe({
      next: () => {
        this.rejecting.set(false);
        this.toaster.success('ECommerce::OrganizationSignupQueueRejectSuccess');
        this.router.navigate(['/admin/organization-signups']);
      },
      error: (err) => {
        this.rejecting.set(false);
        const msg = err?.error?.error?.message ?? err?.message ?? 'ECommerce::OrganizationSignupQueueActionError';
        this.toaster.error(msg);
      },
    });
  }

}
