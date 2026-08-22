import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LocalizationPipe, PermissionDirective } from '@abp/ng.core';
import { TENANT_SIGNUP_MANAGE_PERMISSION } from '../app.routes';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [LocalizationPipe, PermissionDirective, RouterLink],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.scss'],
})
export class AdminDashboardComponent {
  readonly tenantSignupManagePolicy = TENANT_SIGNUP_MANAGE_PERMISSION;
}
