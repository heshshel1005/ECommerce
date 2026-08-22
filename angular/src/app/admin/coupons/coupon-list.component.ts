import { Component, inject, OnInit, signal } from '@angular/core';
import { DatePipe, DecimalPipe } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  CouponAdminService,
  CouponDto,
  CreateCouponDto,
  PagedResultDto,
} from './coupon-admin.service';

const COUPON_TYPE_PERCENT = 0;
const COUPON_TYPE_FIXED = 1;

@Component({
  selector: 'app-coupon-list',
  standalone: true,
  imports: [DatePipe, DecimalPipe, ReactiveFormsModule, LocalizationPipe],
  templateUrl: './coupon-list.component.html',
})
export class CouponListComponent implements OnInit {
  private readonly couponAdminService = inject(CouponAdminService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterService);

  items = signal<CouponDto[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  createForm!: FormGroup;
  showCreateForm = signal(false);
  submitting = signal(false);

  readonly typePercent = COUPON_TYPE_PERCENT;
  readonly typeFixed = COUPON_TYPE_FIXED;

  ngOnInit(): void {
    this.buildForm();
    this.load();
  }

  private buildForm(): void {
    this.createForm = this.fb.group({
      code: ['', [Validators.required, Validators.maxLength(64)]],
      type: [COUPON_TYPE_PERCENT, Validators.required],
      value: [10, [Validators.required, Validators.min(0)]],
      minOrderAmount: [0, [Validators.required, Validators.min(0)]],
      validFrom: [null as string | null],
      validTo: [null as string | null],
      totalUsageLimit: [null as number | null],
      perUserUsageLimit: [null as number | null],
      isActive: [true],
    });
  }

  load(): void {
    this.loading.set(true);
    this.couponAdminService.getList({ maxResultCount: 50, sorting: 'Code' }).subscribe({
      next: (res) => {
        const data = res as unknown as PagedResultDto<CouponDto>;
        const raw = data?.items ?? (res as unknown as Record<string, unknown>)?.items;
        const list = Array.isArray(raw) ? raw : [];
        const total = data?.totalCount ?? (res as unknown as Record<string, number>)?.totalCount ?? 0;
        this.items.set(list);
        this.totalCount.set(total);
        this.loading.set(false);
      },
      error: () => {
        this.items.set([]);
        this.totalCount.set(0);
        this.loading.set(false);
      },
    });
  }

  toggleCreateForm(): void {
    this.showCreateForm.update((v) => !v);
  }

  submitCreate(): void {
    if (this.createForm.invalid || this.submitting()) return;
    const v = this.createForm.value;
    const body: CreateCouponDto = {
      code: String(v.code ?? '').trim().toUpperCase(),
      type: Number(v.type) as 0 | 1,
      value: Number(v.value),
      minOrderAmount: Number(v.minOrderAmount),
      validFrom: v.validFrom ? String(v.validFrom) : null,
      validTo: v.validTo ? String(v.validTo) : null,
      totalUsageLimit: v.totalUsageLimit != null && v.totalUsageLimit !== '' ? Number(v.totalUsageLimit) : null,
      perUserUsageLimit: v.perUserUsageLimit != null && v.perUserUsageLimit !== '' ? Number(v.perUserUsageLimit) : null,
      isActive: !!v.isActive,
    };
    this.submitting.set(true);
    this.couponAdminService.create(body).subscribe({
      next: () => {
        this.submitting.set(false);
        this.toaster.success('ECommerce::CouponCreated', '');
        this.createForm.reset({ type: COUPON_TYPE_PERCENT, value: 10, minOrderAmount: 0, isActive: true });
        this.showCreateForm.set(false);
        this.load();
      },
      error: (err) => {
        this.submitting.set(false);
        const msg = err?.error?.error?.message ?? err?.message ?? 'Failed to create coupon.';
        this.toaster.error(msg);
      },
    });
  }

  typeLabel(type: number): string {
    return type === COUPON_TYPE_PERCENT ? 'ECommerce::CouponTypePercent' : 'ECommerce::CouponTypeFixed';
  }

  formatDiscount(c: CouponDto): string {
    if (c.type === COUPON_TYPE_PERCENT) return `${c.value}%`;
    return `${c.value}`;
  }
}
