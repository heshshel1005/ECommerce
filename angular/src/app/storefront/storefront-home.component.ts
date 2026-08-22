import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LocalizationPipe } from '@abp/ng.core';

@Component({
  selector: 'app-storefront-home',
  templateUrl: './storefront-home.component.html',
  styleUrls: ['./storefront-home.component.scss'],
  imports: [LocalizationPipe, RouterLink],
})
export class StorefrontHomeComponent {}
