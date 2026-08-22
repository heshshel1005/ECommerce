import { Component, inject, OnInit, signal } from '@angular/core';
import { NgFor, NgIf } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import {
  CmsAdminService,
  BlogDto,
  BlogPostListDto,
  CreateBlogPostDto,
  PagedResultDto,
  UpdateBlogPostDto,
} from './cms-admin.service';

@Component({
  selector: 'app-cms-blog-post-list',
  standalone: true,
  imports: [NgFor, NgIf, ReactiveFormsModule, LocalizationPipe, RouterLink],
  templateUrl: './blog-post-list.component.html',
})
export class CmsBlogPostListComponent implements OnInit {
  private readonly cms = inject(CmsAdminService);
  private readonly route = inject(ActivatedRoute);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterService);

  blogId = signal<string>('');
  blogName = signal<string>('');
  posts = signal<BlogPostListDto[]>([]);
  loading = signal(true);
  showForm = signal(false);
  editingId = signal<string | null>(null);
  submitting = signal(false);
  form!: FormGroup;

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('blogId');
    if (id) this.blogId.set(id);
    this.buildForm();
    this.loadBlog();
    this.load();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(512)]],
      slug: ['', [Validators.required, Validators.maxLength(256)]],
      shortDescription: [''],
      content: [''],
    });
  }

  private loadBlog(): void {
    const id = this.blogId();
    if (!id) return;
    this.cms.getBlogs().subscribe({
      next: (res) => {
        const list = (res as { items?: BlogDto[] })?.items ?? [];
        const blog = list.find((b) => b.id === id);
        if (blog) this.blogName.set(blog.name);
      },
    });
  }

  load(): void {
    const id = this.blogId();
    if (!id) {
      this.loading.set(false);
      return;
    }
    this.loading.set(true);
    this.cms.getBlogPosts({ blogId: id, maxResultCount: 100 }).subscribe({
      next: (res) => {
        const data = res as unknown as PagedResultDto<BlogPostListDto>;
        this.posts.set(data?.items ?? []);
        this.loading.set(false);
      },
      error: () => {
        this.posts.set([]);
        this.loading.set(false);
      },
    });
  }

  openCreate(): void {
    this.editingId.set(null);
    this.form.reset({ title: '', slug: '', shortDescription: '', content: '' });
    this.showForm.set(true);
  }

  openEdit(post: BlogPostListDto): void {
    this.editingId.set(post.id);
    this.form.patchValue({
      title: post.title,
      slug: post.slug,
      shortDescription: post.shortDescription ?? '',
      content: post.content ?? '',
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
    const blogId = this.blogId();
    const value = this.form.getRawValue();
    this.submitting.set(true);
    if (id) {
      this.cms
        .updateBlogPost(id, {
          title: value.title,
          slug: value.slug,
          shortDescription: value.shortDescription || null,
          content: value.content || null,
        } as UpdateBlogPostDto)
        .subscribe({
          next: () => {
            this.toaster.success('ECommerce::PostUpdated');
            this.cancelForm();
            this.load();
            this.submitting.set(false);
          },
          error: () => this.submitting.set(false),
        });
    } else {
      this.cms
        .createBlogPost({
          blogId,
          title: value.title,
          slug: value.slug,
          shortDescription: value.shortDescription || null,
          content: value.content || null,
        } as CreateBlogPostDto)
        .subscribe({
          next: () => {
            this.toaster.success('ECommerce::PostCreated');
            this.cancelForm();
            this.load();
            this.submitting.set(false);
          },
          error: () => this.submitting.set(false),
        });
    }
  }
}
