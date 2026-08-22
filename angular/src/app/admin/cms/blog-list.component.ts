import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { RouterLink } from '@angular/router';
import { CmsAdminService, BlogDto, CreateBlogDto } from './cms-admin.service';

@Component({
  selector: 'app-cms-blog-list',
  standalone: true,
  imports: [NgFor, NgIf, ReactiveFormsModule, LocalizationPipe, RouterLink],
  templateUrl: './blog-list.component.html',
})
export class CmsBlogListComponent implements OnInit {
  private readonly cms = inject(CmsAdminService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterService);

  blogs = signal<BlogDto[]>([]);
  loading = signal(true);
  showForm = signal(false);
  submitting = signal(false);
  form!: FormGroup;

  ngOnInit(): void {
    this.buildForm();
    this.load();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(256)]],
      slug: ['', [Validators.required, Validators.maxLength(256)]],
    });
  }

  load(): void {
    this.loading.set(true);
    this.cms.getBlogs().subscribe({
      next: (res) => {
        const list = (res as { items?: BlogDto[] })?.items ?? [];
        this.blogs.set(list);
        this.loading.set(false);
      },
      error: () => {
        this.blogs.set([]);
        this.loading.set(false);
      },
    });
  }

  openCreate(): void {
    this.form.reset({ name: '', slug: '' });
    this.showForm.set(true);
  }

  cancelForm(): void {
    this.showForm.set(false);
  }

  submit(): void {
    if (this.form.invalid) return;
    const value = this.form.getRawValue() as CreateBlogDto;
    this.submitting.set(true);
    this.cms.createBlog(value).subscribe({
      next: () => {
        this.toaster.success('ECommerce::BlogCreated');
        this.cancelForm();
        this.load();
        this.submitting.set(false);
      },
      error: () => this.submitting.set(false),
    });
  }
}
