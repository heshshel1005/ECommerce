import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';

@Component({
  selector: 'app-admin-home',
  standalone: true,
  imports: [LocalizationPipe, RouterLink],
  templateUrl: './admin-home.component.html',
  styleUrls: ['./admin-home.component.scss'],
})
export class AdminHomeComponent {}
