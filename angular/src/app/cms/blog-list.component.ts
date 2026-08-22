import { AsyncPipe, DatePipe, NgFor, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

interface BlogPostDto {
  id: string;
  title: string;
  slug: string;
  shortDescription?: string | null;
  creationTime: string;
}

@Component({
  selector: 'app-blog-list',
  standalone: true,
  imports: [NgIf, NgFor, AsyncPipe, DatePipe],
  templateUrl: './blog-list.component.html',
})
export class BlogListComponent implements OnInit {
  private readonly http = inject(HttpClient);

  loading = true;
  posts: BlogPostDto[] = [];

  ngOnInit(): void {
    // Default blog slug "default". Adjust if you use a different blog slug.
    this.http
      .get<BlogPostDto[]>(`/api/cms-kit-public/blogs/default/posts`)
      .subscribe({
        next: posts => {
          this.posts = posts;
          this.loading = false;
        },
        error: () => {
          this.posts = [];
          this.loading = false;
        },
      });
  }
}

