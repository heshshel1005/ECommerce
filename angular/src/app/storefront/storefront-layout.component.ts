import { Component, inject, signal, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService, LocalizationPipe } from '@abp/ng.core';
import { BreadcrumbsComponent } from '../shared/breadcrumbs/breadcrumbs.component';
import { CartService } from './cart.service';
import { LanguageSwitcherComponent } from './language-switcher.component';

@Component({
  selector: 'app-storefront-layout',
  templateUrl: './storefront-layout.component.html',
  styleUrls: ['./storefront-layout.component.scss'],
  imports: [RouterOutlet, RouterLink, RouterLinkActive, LocalizationPipe, BreadcrumbsComponent, LanguageSwitcherComponent],
})
export class StorefrontLayoutComponent implements OnInit {
  protected readonly authService = inject(AuthService);
  protected readonly cartService = inject(CartService);

  ngOnInit(): void {
    this.cartService.getCart().subscribe({ error: () => {} });
  }
  currentYear = new Date().getFullYear();
  navOpen = signal(false);
  accountMenuOpen = signal(false);

  toggleNav() {
    this.navOpen.update((v) => !v);
  }

  closeNav() {
    this.navOpen.set(false);
  }

  toggleAccountMenu() {
    this.accountMenuOpen.update((v) => !v);
  }

  closeAccountMenu() {
    this.accountMenuOpen.set(false);
  }

  logout() {
    this.authService.logout();
    this.closeAccountMenu();
    this.closeNav();
  }
}
