import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatDividerModule } from '@angular/material/divider';

import { LoanService } from '../../../app/services/loan-service';
import { LoanType } from '../../../app/models/loan-type.model';

/* ================= SUCCESS DIALOG ================= */

@Component({
  selector: 'app-loan-success-dialog',
  standalone: true,
  imports: [MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Success</h2>

    <mat-dialog-content>
      <p>Loan applied successfully </p>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-raised-button color="primary" mat-dialog-close>
        OK
      </button>
    </mat-dialog-actions>
  `
})
export class LoanSuccessDialogComponent {}

/* ================= APPLY LOAN COMPONENT ================= */

@Component({
  selector: 'app-apply-loan',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDialogModule,
    MatDividerModule,
    LoanSuccessDialogComponent
  ],
  template: `
    <mat-card class="loan-card">
      <h2>Apply for Loan</h2>

      <form [formGroup]="loanForm" (ngSubmit)="applyLoan()">

        <!-- Loan Type -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Loan Type</mat-label>
          <mat-select
            formControlName="loanTypeId"
            (selectionChange)="onLoanTypeChange($event.value)">
            <mat-option *ngFor="let loan of loanTypes" [value]="loan.loanTypeId">
              {{ loan.loanName }}
            </mat-option>
          </mat-select>
        </mat-form-field>

        <!-- Loan Info -->
        <div *ngIf="selectedLoanType" class="loan-info">
          <p><strong>ROI:</strong> {{ selectedLoanType.roi }}%</p>
          <p><strong>Max Tenure:</strong> {{ selectedLoanType.maxTenure }} months</p>
          <p>
            <strong>Amount Range:</strong>
            {{ selectedLoanType.minAmount }} – {{ selectedLoanType.maxAmount }}
          </p>
        </div>

        <!-- Amount -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Loan Amount</mat-label>
          <input matInput type="number" formControlName="amount">
        </mat-form-field>

        <!-- Tenure -->
        <mat-form-field appearance="outline" class="full-width">
          <mat-label>Tenure (months)</mat-label>
          <input matInput type="number" formControlName="tenure">
        </mat-form-field>

        <button
          mat-raised-button
          color="primary"
          type="submit"
          [disabled]="loanForm.invalid">
          Apply Loan
        </button>

      </form>
    </mat-card>
  `,
  styles: [`
    .loan-card {
      max-width: 520px;
      margin: 24px auto;
      padding: 24px;
    }
    .full-width {
      width: 100%;
      margin-bottom: 16px;
    }
    .loan-info {
      background: #f5f7fa;
      padding: 12px;
      border-radius: 6px;
      margin-bottom: 16px;
    }
  `]
})
export class ApplyLoanComponent implements OnInit {

  loanForm!: FormGroup;
  loanTypes: LoanType[] = [];
  selectedLoanType: LoanType | null = null;

  constructor(
    private fb: FormBuilder,
    private loanService: LoanService,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.loanForm = this.fb.group({
      loanTypeId: ['', Validators.required],
      amount: ['', Validators.required],
      tenure: ['', Validators.required]
    });

    this.loadLoanTypes();
  }

  loadLoanTypes(): void {
    this.loanService.getLoanTypes().subscribe(res => {
      this.loanTypes = res;
    });
  }

  onLoanTypeChange(id: number): void {
    this.selectedLoanType =
      this.loanTypes.find(l => l.loanTypeId === id) ?? null;

    if (!this.selectedLoanType) return;

    this.loanForm.get('tenure')?.setValidators([
      Validators.required,
      Validators.min(1),
      Validators.max(this.selectedLoanType.maxTenure)
    ]);

    this.loanForm.get('amount')?.setValidators([
      Validators.required,
      Validators.min(this.selectedLoanType.minAmount),
      Validators.max(this.selectedLoanType.maxAmount)
    ]);

    this.loanForm.get('tenure')?.updateValueAndValidity();
    this.loanForm.get('amount')?.updateValueAndValidity();
  }

  applyLoan(): void {
  if (this.loanForm.invalid) return;

  if (!this.selectedLoanType) return;

  const payload = {
    LoanTypeId: Number(this.loanForm.value.loanTypeId),
    Amount: Number(this.loanForm.value.amount),
    Tenure: Number(this.loanForm.value.tenure)
  };

  this.loanService.applyLoan(payload).subscribe({
    next: () => {
      this.dialog.open(LoanSuccessDialogComponent, { width: '350px' });
      this.loanForm.reset();
      this.selectedLoanType = null;
    },
    error: err => {
      const message = err?.error?.message || 'Loan application failed';
      alert(message); 
    }
  });
}

}
