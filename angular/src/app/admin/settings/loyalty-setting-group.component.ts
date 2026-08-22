import { Component, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { LocalizationPipe } from '@abp/ng.core';
import { ToasterService } from '@abp/ng.theme.shared';
import { ButtonComponent } from '@abp/ng.theme.shared';
import { LoyaltySettingsService } from './loyalty-settings.service';
import { finalize } from 'rxjs';

@Component({
  selector: 'app-loyalty-setting-group',
  standalone: true,
  imports: [ReactiveFormsModule, LocalizationPipe, ButtonComponent],
  templateUrl: './loyalty-setting-group.component.html',
})
export class LoyaltySettingGroupComponent implements OnInit {
  private readonly loyaltySettingsService = inject(LoyaltySettingsService);
  private readonly fb = inject(FormBuilder);
  private readonly toaster = inject(ToasterService);

  form!: FormGroup;
  saving = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      pointsPerCurrencyUnit: ['1', [Validators.required, Validators.min(1), Validators.pattern(/^\d+$/)]],
    });
    this.load();
  }

  load(): void {
    this.loyaltySettingsService.get().subscribe(settings => {
      this.form.patchValue({
        pointsPerCurrencyUnit: settings.pointsPerCurrencyUnit || '1',
      });
    });
  }

  submit(): void {
    if (this.form.invalid || this.saving) return;
    this.saving = true;
    const raw = this.form.getRawValue();
    const body = {
      pointsPerCurrencyUnit: String(raw?.pointsPerCurrencyUnit ?? '1'),
    };
    this.loyaltySettingsService
      .update(body)
      .pipe(finalize(() => (this.saving = false)))
      .subscribe(() => {
        this.toaster.success('AbpSettingManagement::SavedSuccessfully');
        this.load();
      });
  }
}
