import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  NewsletterAdminService,
  NewsletterSubscriberDto,
  PagedResultDto,
  SendNewsletterCampaignDto,
} from './newsletter-admin.service';

const PAGE_SIZE = 20;

@Component({
  selector: 'app-newsletter-admin',
  standalone: true,
  imports: [DatePipe, ReactiveFormsModule, LocalizationPipe],
  templateUrl: './newsletter-admin.component.html',
})
export class NewsletterAdminComponent implements OnInit {
  private readonly newsletterAdmin = inject(NewsletterAdminService);
  private readonly toaster = inject(ToasterService);
  private readonly fb = inject(FormBuilder);

  subscribers = signal<NewsletterSubscriberDto[]>([]);
  totalCount = signal(0);
  loading = signal(true);
  activeOnly = signal<boolean | null>(null);
  currentPage = signal(0);
  sendForm = this.fb.group({
    subject: ['', [Validators.required, Validators.maxLength(500)]],
    body: ['', [Validators.required]],
    isBodyHtml: [true],
  });
  sending = signal(false);

  totalPages = computed(() => Math.max(1, Math.ceil(this.totalCount() / PAGE_SIZE)));
  hasPrev = computed(() => this.currentPage() > 0);
  hasNext = computed(() => this.currentPage() < this.totalPages() - 1);

  ngOnInit(): void {
    this.loadSubscribers();
  }

  loadSubscribers(): void {
    this.loading.set(true);
    const skip = this.currentPage() * PAGE_SIZE;
    const params: { skipCount: number; maxResultCount: number; sorting?: string; isActiveOnly?: boolean } = {
      skipCount: skip,
      maxResultCount: PAGE_SIZE,
      sorting: 'Email',
    };
    if (this.activeOnly() !== null) {
      params.isActiveOnly = this.activeOnly() ?? undefined;
    }
    this.newsletterAdmin.getSubscribers(params).subscribe({
      next: (res) => {
        const data = res as unknown as PagedResultDto<NewsletterSubscriberDto>;
        const raw = data?.items ?? (res as unknown as Record<string, unknown>)?.items;
        const list = Array.isArray(raw) ? raw : [];
        const total = data?.totalCount ?? (res as unknown as Record<string, number>)?.totalCount ?? 0;
        this.subscribers.set(list);
        this.totalCount.set(total);
        this.loading.set(false);
      },
      error: () => {
        this.subscribers.set([]);
        this.totalCount.set(0);
        this.loading.set(false);
      },
    });
  }

  onActiveOnlyChange(value: string): void {
    if (value === '') {
      this.activeOnly.set(null);
    } else {
      this.activeOnly.set(value === 'true');
    }
    this.currentPage.set(0);
    this.loadSubscribers();
  }

  goPrev(): void {
    if (this.hasPrev()) {
      this.currentPage.update((p) => p - 1);
      this.loadSubscribers();
    }
  }

  goNext(): void {
    if (this.hasNext()) {
      this.currentPage.update((p) => p + 1);
      this.loadSubscribers();
    }
  }

  sendCampaign(): void {
    if (this.sendForm.invalid || this.sending()) return;
    const v = this.sendForm.value;
    const dto: SendNewsletterCampaignDto = {
      subject: String(v.subject ?? '').trim(),
      body: String(v.body ?? ''),
      isBodyHtml: !!v.isBodyHtml,
    };
    this.sending.set(true);
    this.newsletterAdmin.sendCampaign(dto).subscribe({
      next: () => {
        this.sending.set(false);
        this.toaster.success('ECommerce::NewsletterSendSuccess');
        this.sendForm.patchValue({ subject: '', body: '' });
      },
      error: (err) => {
        this.sending.set(false);
        const msg = err?.error?.error?.message ?? err?.message ?? 'ECommerce::Error';
        this.toaster.error(msg);
      },
    });
  }
}
