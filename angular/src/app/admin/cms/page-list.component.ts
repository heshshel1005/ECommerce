import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  CmsAdminService,
  CreatePageInputDto,
  PageDto,
  PagedResultDto,
  UpdatePageInputDto,
} from './cms-admin.service';

@Component({
  selector: 'app-cms-page-list',
  standalone: true,
  imports: [NgFor, NgIf, ReactiveFormsModule, LocalizationPipe],
  templateUrl: './page-list.component.html',
})
export class CmsPageListComponent implements OnInit {
  private readonly cms = inject(CmsAdminService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterService);

  items = signal<PageDto[]>([]);
  loading = signal(true);
  showForm = signal(false);
  editingId = signal<string | null>(null);
  submitting = signal(false);
  form!: FormGroup;

  ngOnInit(): void {
    this.buildForm();
    this.load();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(256)]],
      slug: ['', [Validators.required, Validators.maxLength(256)]],
      content: [''],
    });
  }

  load(): void {
    this.loading.set(true);
    this.cms.getPages({ maxResultCount: 100 }).subscribe({
      next: (res) => {
        const data = res as unknown as PagedResultDto<PageDto>;
        const list = data?.items ?? [];
        this.items.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.items.set([]);
        this.loading.set(false);
      },
    });
  }

  openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ title: '', slug: '', content: '' });
    this.showForm.set(true);
  }

  openEdit(page: PageDto): void {
    this.editingId.set(page.id);
    this.form.patchValue({
      title: page.title,
      slug: page.slug,
      content: page.content ?? '',
    });
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
    this.editingId.set(null);
  }

  submit(): void {
    if (this.form.invalid) return;
    const id = this.editingId();
    const value = this.form.getRawValue() as CreatePageInputDto;
    this.submitting.set(true);
    const req = id
      ? this.cms.updatePage(id, value as UpdatePageInputDto)
      : this.cms.createPage(value);
    req.subscribe({
      next: () => {
        this.toaster.success(id ? 'ECommerce::PageUpdated' : 'ECommerce::PageCreated');
        this.cancelForm();
        this.load();
        this.submitting.set(false);
      },
      error: () => this.submitting.set(false),
    });
  }

  viewOnStorefront(slug: string): string {
    return `/page/${slug}`;
  }
}
