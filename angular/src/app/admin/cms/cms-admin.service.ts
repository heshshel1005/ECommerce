import { Injectable, inject } from '@angular/core';
import { RestService } from '@abp/ng.core';
import { Observable } from 'rxjs';

const BASE = '/api/cms-kit-admin';

export interface PageDto {
  id: string;
  title: string;
  slug: string;
  content?: string | null;
  script?: string | null;
  style?: string | null;
  status?: number;
  creationTime?: string;
}

export interface CreatePageInputDto {
  title: string;
  slug: string;
  content?: string | null;
  script?: string | null;
  style?: string | null;
}

export interface UpdatePageInputDto extends CreatePageInputDto {}

export interface BlogDto {
  id: string;
  name: string;
  slug: string;
}

export interface BlogPostListDto {
  id: string;
  blogId: string;
  title: string;
  slug: string;
  shortDescription?: string | null;
  content?: string | null;
  status?: number;
  creationTime?: string;
}

export interface CreateBlogDto {
  name: string;
  slug: string;
}

export interface CreateBlogPostDto {
  blogId: string;
  title: string;
  slug: string;
  shortDescription?: string | null;
  content?: string | null;
}

export interface UpdateBlogPostDto {
  title: string;
  slug: string;
  shortDescription?: string | null;
  content?: string | null;
}

export interface PagedResultDto<T> {
  totalCount: number;
  items: T[];
}

@Injectable({ providedIn: 'root' })
export class CmsAdminService {
  private readonly rest = inject(RestService);

  // Pages
  getPages(params: { skipCount?: number; maxResultCount?: number; sorting?: string } = {}): Observable<PagedResultDto<PageDto>> {
    return this.rest.request<void, PagedResultDto<PageDto>>({
      method: 'GET',
      url: `${BASE}/pages`,
      params: {
        SkipCount: String(params.skipCount ?? 0),
        MaxResultCount: String(params.maxResultCount ?? 50),
        Sorting: params.sorting ?? 'CreationTime DESC',
      },
    });
  }

  getPage(id: string): Observable<PageDto> {
    return this.rest.request<void, PageDto>({
      method: 'GET',
      url: `${BASE}/pages/${id}`,
    });
  }

  createPage(body: CreatePageInputDto): Observable<PageDto> {
    return this.rest.request<CreatePageInputDto, PageDto>({
      method: 'POST',
      url: `${BASE}/pages`,
      body,
    });
  }

  updatePage(id: string, body: UpdatePageInputDto): Observable<PageDto> {
    return this.rest.request<UpdatePageInputDto, PageDto>({
      method: 'PUT',
      url: `${BASE}/pages/${id}`,
      body,
    });
  }

  deletePage(id: string): Observable<void> {
    return this.rest.request<void, void>({
      method: 'DELETE',
      url: `${BASE}/pages/${id}`,
    });
  }

  // Blogs
  getBlogs(): Observable<{ items: BlogDto[] }> {
    return this.rest.request<void, { items: BlogDto[] }>({
      method: 'GET',
      url: `${BASE}/blogs/all`,
    });
  }

  createBlog(body: CreateBlogDto): Observable<BlogDto> {
    return this.rest.request<CreateBlogDto, BlogDto>({
      method: 'POST',
      url: `${BASE}/blogs`,
      body,
    });
  }

  getBlogPosts(params: { blogId: string; skipCount?: number; maxResultCount?: number }): Observable<PagedResultDto<BlogPostListDto>> {
    return this.rest.request<void, PagedResultDto<BlogPostListDto>>({
      method: 'GET',
      url: `${BASE}/blogs/blog-posts`,
      params: {
        BlogId: params.blogId,
        SkipCount: String(params.skipCount ?? 0),
        MaxResultCount: String(params.maxResultCount ?? 50),
      },
    });
  }

  getBlogPost(id: string): Observable<BlogPostListDto> {
    return this.rest.request<void, BlogPostListDto>({
      method: 'GET',
      url: `${BASE}/blogs/blog-posts/${id}`,
    });
  }

  createBlogPost(body: CreateBlogPostDto): Observable<BlogPostListDto> {
    return this.rest.request<CreateBlogPostDto, BlogPostListDto>({
      method: 'POST',
      url: `${BASE}/blogs/blog-posts`,
      body,
    });
  }

  updateBlogPost(id: string, body: UpdateBlogPostDto): Observable<BlogPostListDto> {
    return this.rest.request<UpdateBlogPostDto, BlogPostListDto>({
      method: 'PUT',
      url: `${BASE}/blogs/blog-posts/${id}`,
      body,
    });
  }

  publishBlogPost(id: string): Observable<BlogPostListDto> {
    return this.rest.request<void, BlogPostListDto>({
      method: 'PUT',
      url: `${BASE}/blogs/blog-posts/${id}/publish`,
    });
  }
}
