import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { DatePipe } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  OrganizationSignupHostService,
  OrganizationSignupRequestDto,
  PagedResultDto,
  RepairTenantAdminPermissionsResultDto,
} from './organization-signup-host.service';

const PAGE_SIZE = 20;

const SIGNUP_STATUS_NAMES = ['Pending', 'Approved', 'Rejected'] as const;

@Component({
  selector: 'app-organization-signup-queue-list',
  standalone: true,
  imports: [DatePipe, RouterLink, LocalizationPipe],
  templateUrl: './organization-signup-queue-list.component.html',
})
export class OrganizationSignupQueueListComponent implements OnInit {
  private readonly hostService = inject(OrganizationSignupHostService);
  private readonly toaster = inject(ToasterService);

  items = signal<OrganizationSignupRequestDto[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  repairingPermissions = signal(false);
  /** '' | '0' | '1' | '2' for filter select */
  statusFilter = signal('');
  currentPage = signal(0);

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / PAGE_SIZE)));
  hasPrev = computed(() => this.currentPage() > 0);
  hasNext = computed(() => this.currentPage() < this.totalPages() - 1);

  ngOnInit(): void {
    this.load();
  }

  statusLabel(status: number): string {
    const name = SIGNUP_STATUS_NAMES[status];
    return name ? `ECommerce::Enum:OrganizationSignupStatus.${name}` : 'ECommerce::NoData';
  }

  onFilterChange(): void {
    this.currentPage.set(0);
    this.load();
  }

  load(): void {
    this.loading.set(true);
    const skip = this.currentPage() * PAGE_SIZE;
    const sf = this.statusFilter();
    const params: {
      skipCount: number;
      maxResultCount: number;
      sorting: string;
      status?: number;
    } = {
      skipCount: skip,
      maxResultCount: PAGE_SIZE,
      sorting: 'CreationTime desc',
    };
    if (sf !== '') {
      params.status = Number(sf);
    }
    this.hostService.getList(params).subscribe({
      next: (res) => {
        const data = res as unknown as PagedResultDto<OrganizationSignupRequestDto>;
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
        this.toaster.error('ECommerce::OrganizationSignupQueueLoadError', 'Error');
      },
    });
  }

  goPrev(): void {
    if (this.hasPrev()) {
      this.currentPage.update((p) => p - 1);
      this.load();
    }
  }

  goNext(): void {
    if (this.hasNext()) {
      this.currentPage.update((p) => p + 1);
      this.load();
    }
  }

  repairTenantAdminPermissions(): void {
    if (this.repairingPermissions()) return;
    if (!window.confirm('Run one-click repair for admin permissions on all existing tenants?')) return;

    this.repairingPermissions.set(true);
    this.hostService.repairTenantAdminPermissions().subscribe({
      next: (res) => {
        const result = res as RepairTenantAdminPermissionsResultDto;
        this.repairingPermissions.set(false);
        if (result.failedTenants > 0) {
          this.toaster.warn(
            `Repaired ${result.repairedTenants}/${result.totalTenants} tenants. Failed: ${result.failedTenants}.`,
            'Warning',
          );
        } else {
          this.toaster.success(
            `Repaired admin permissions for ${result.repairedTenants} tenants.`,
            'Success',
          );
        }
      },
      error: () => {
        this.repairingPermissions.set(false);
        this.toaster.error('ECommerce::OrganizationSignupQueueActionError', 'Error');
      },
    });
  }
}
