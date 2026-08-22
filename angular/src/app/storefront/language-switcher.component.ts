import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ConfigStateService, SessionStateService } from '@abp/ng.core';

@Component({
  selector: 'app-language-switcher',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (languages().length > 1) {
      <div class="storefront-language-switcher position-relative">
        <button
          type="button"
          class="storefront-nav-link storefront-nav-dropdown-toggle"
          [attr.aria-expanded]="open()"
          aria-haspopup="true"
          (click)="toggle()"
        >
          <i class="fa fa-globe me-1" aria-hidden="true"></i>
          <span>{{ currentDisplayName() }}</span>
          <i class="fa fa-chevron-down ms-1 small" [class.fa-rotate-180]="open()" aria-hidden="true"></i>
        </button>
        @if (open()) {
          <ul class="dropdown-menu dropdown-menu-end show">
            @for (lang of languages(); track lang.cultureName) {
              <li>
                <button
                  type="button"
                  class="dropdown-item"
                  [class.active]="currentCulture() === lang.cultureName"
                  (click)="setLanguage(lang.cultureName)"
                >
                  {{ lang.displayName ?? lang.cultureName ?? lang.twoLetterISOLanguageName }}
                </button>
              </li>
            }
          </ul>
        }
      </div>
    }
  `,
  styles: [
    `
      .storefront-language-switcher .dropdown-menu {
        --bs-dropdown-link-color: #212529;
        position: absolute;
        top: 100%;
        right: 0;
        margin-top: 0.25rem;
        min-width: 8rem;
        z-index: 1050;
      }
    `,
  ],
})
export class LanguageSwitcherComponent implements OnInit {
  private readonly configState = inject(ConfigStateService);
  private readonly sessionState = inject(SessionStateService);

  open = signal(false);
  languages = signal<{ cultureName?: string; displayName?: string; twoLetterISOLanguageName?: string }[]>([]);
  currentCulture = signal('');

  currentDisplayName = computed(() => {
    const lang = this.currentCulture();
    const list = this.languages();
    const found = list.find((l) => (l.cultureName ?? '') === lang);
    return found?.displayName ?? found?.cultureName ?? found?.twoLetterISOLanguageName ?? (lang || 'Language');
  });

  ngOnInit(): void {
    const localization = this.configState.getOne('localization');
    const langs = localization?.languages ?? [];
    this.languages.set(langs);
    this.currentCulture.set(this.sessionState.getLanguage() ?? '');
    this.sessionState.getLanguage$().subscribe((lang) => this.currentCulture.set(lang ?? ''));
  }

  toggle(): void {
    this.open.update((v) => !v);
  }

  setLanguage(cultureName: string | undefined): void {
    if (!cultureName || cultureName === this.currentCulture()) {
      this.open.set(false);
      return;
    }
    this.sessionState.setLanguage(cultureName);
    this.open.set(false);
    window.location.reload();
  }
}
