import { AsyncPipe, NgIf } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { HttpClient } from '@angular/common/http';

interface CmsPageDto {
  title: string;
  slug: string;
  content: string;
}

@Component({
  selector: 'app-cms-page',
  standalone: true,
  imports: [NgIf, AsyncPipe],
  templateUrl: './cms-page.component.html',
})
export class CmsPageComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly http = inject(HttpClient);

  loading = true;
  page?: CmsPageDto;

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const slug = params.get('slug') ?? 'home';
      this.loadPage(slug);
    });
  }

  private loadPage(slug: string): void {
    this.loading = true;
    this.http
      .get<CmsPageDto>(`/api/cms-kit-public/pages/by-slug`, {
        params: { slug },
      })
      .subscribe({
        next: page => {
          this.page = page;
          this.loading = false;
        },
        error: () => {
          this.page = undefined;
          this.loading = false;
        },
      });
  }
}

