import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { ToasterService } from '@abp/ng.theme.shared';
import { SubscriptionService } from './subscription.service';

@Component({
  standalone: true,
  imports: [RouterLink],
  template: `
    <div class="card shadow-sm mx-auto" style="max-width: 28rem;">
      <div class="card-body p-4 text-center">
        @if (status === 'loading') {
          <p class="mb-0">Confirming your email…</p>
          <div class="spinner-border text-primary mt-2" role="status">
            <span class="visually-hidden">Loading...</span>
          </div>
        } @else if (status === 'success') {
          <p class="text-success mb-2">Your email has been confirmed.</p>
          <a class="btn btn-primary" routerLink="/account/login">Log in</a>
        } @else {
          <p class="text-danger mb-2">{{ errorMessage }}</p>
          <a class="btn btn-outline-primary" routerLink="/account/login">Log in</a>
        }
      </div>
    </div>
  `,
})
export class ConfirmEmailComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly toaster = inject(ToasterService);
  private readonly subscription = inject(SubscriptionService);

  status: 'loading' | 'success' | 'error' = 'loading';
  errorMessage = '';

  ngOnInit(): void {
    const userId = this.route.snapshot.queryParamMap.get('userId');
    const token = this.route.snapshot.queryParamMap.get('token');

    if (!userId || !token) {
      this.status = 'error';
      this.errorMessage = 'Invalid confirmation link. Missing userId or token.';
      return;
    }

    this.subscription.confirmEmail(userId, token).subscribe({
      next: () => {
        this.status = 'success';
        this.toaster.success('Your email has been confirmed.', '', { life: 5000 });
      },
      error: (err) => {
        this.status = 'error';
        this.errorMessage =
          err?.error?.error?.message ??
          err?.error?.error_description ??
          (typeof err?.message === 'string' ? err.message : 'Failed to confirm email. The link may have expired.');
        this.toaster.error(this.errorMessage, 'Email confirmation failed', { life: 7000 });
      },
    });
  }
}
