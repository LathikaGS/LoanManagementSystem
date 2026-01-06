import { Component, signal, computed, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatTableModule } from '@angular/material/table';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatIconModule } from '@angular/material/icon';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';

import { EmiService } from '../../../app/services/emi.service';
import { Loan } from '../../../app/models/loan.model';
import { LoanStatus } from '../../../app/models/loan-status.model';

interface LoanStatusCount {
  status: LoanStatus;
  count: number;
}

@Component({
  selector: 'app-customer-emi',
  standalone: true,
  templateUrl: './customer-emi.html',
  styleUrls: ['./customer-emi.css'],
  imports: [
    CommonModule,
    FormsModule,
    MatCardModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatTableModule,
    MatExpansionModule,
    MatIconModule,
    MatChipsModule,
    MatSnackBarModule,
    MatDialogModule
  ]
})
export class CustomerEmiComponent {

  loans = signal<Loan[]>([]);
  loading = signal(true);
  loadingEmis = signal<number | null>(null);

  displayedColumns = ['date', 'amount', 'status', 'action'];

  // 👇 expose enum to template
  LoanStatus = LoanStatus;

  constructor(
    private emiService: EmiService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    this.fetchLoans();
  }

  // ================= FETCH LOANS =================
  fetchLoans() {
    this.loading.set(true);

    this.emiService.getLoans().subscribe({
      next: loans => {
        const sorted = this.sortLoansByStatus(loans);
        this.loans.set(sorted);
        this.loading.set(false);

        queueMicrotask(() => {
          sorted
            .filter(l => l.status === LoanStatus.Approved)
            .forEach(l => this.fetchEmis(l));
        });
      },
      error: () => {
        this.loading.set(false);
        this.snackBar.open('Failed to load loans', 'Close', { duration: 3000 });
      }
    });
  }

  // ================= SORT =================
  private sortLoansByStatus(loans: Loan[]): Loan[] {
  return [...loans].sort((a, b) => {
    if (a.status === LoanStatus.Approved && b.status !== LoanStatus.Approved) {
      return -1;
    }
    if (a.status !== LoanStatus.Approved && b.status === LoanStatus.Approved) {
      return 1;
    }
    return a.loanId - b.loanId; // stable secondary sort
  });
}


  // ================= STATUS COUNTS =================
  loanStatusCounts = computed<LoanStatusCount[]>(() => {
    const map = new Map<LoanStatus, number>();
    for (const loan of this.loans()) {
      map.set(loan.status, (map.get(loan.status) ?? 0) + 1);
    }
    return Array.from(map.entries()).map(([status, count]) => ({ status, count }));
  });

  // ================= FETCH EMIs =================
  fetchEmis(loan: Loan) {
    if (loan.EMIs) return;

    loan.EMIs = [];
    this.loadingEmis.set(loan.loanId);

    this.emiService.getEmis(loan.loanId).subscribe({
      next: emis => {
        loan.EMIs = emis ?? [];
        this.loadingEmis.set(null);
      },
      error: () => {
        this.loadingEmis.set(null);
        this.snackBar.open('Failed to fetch EMIs', 'Close', { duration: 3000 });
      }
    });
  }

  // ================= PAY EMI =================
  payEmi(loanId: number, emiId: number) {

  // First dialog (Pay Now)
  const payDialog = this.dialog.open(PaymentDialogComponent, {
    width: '420px',
    data: { title: 'Pay EMI' }
  });

  payDialog.afterClosed().subscribe(payNow => {
    if (!payNow) return;

    // Second dialog (Final confirmation)
    const confirmDialog = this.dialog.open(ConfirmPaymentDialogComponent, {
      width: '400px',
      data: { message: 'Are you sure you want to complete this payment?' }
    });

    confirmDialog.afterClosed().subscribe(confirm => {
      if (!confirm) return;

      this.emiService.payEmi(emiId).subscribe({
        next: () => {
          const loan = this.loans().find(l => l.loanId === loanId);
          const emi = loan?.EMIs?.find(e => e.emiId === emiId);
          if (emi) {
            emi.paidStatus = true;
            emi.paidOn = new Date().toISOString(); 
          }

          this.snackBar.open('EMI paid successfully', 'Close', { duration: 3000 });
        },
        error: () =>
          this.snackBar.open('Payment failed', 'Close', { duration: 3000 })
      });
    });
  });
}


  // ================= PAY ALL =================
  payAll(loan: Loan) {
    const dialogRef = this.dialog.open(PaymentDialogComponent, {
      width: '420px',
      data: {
        title: 'Confirm Full Payment',
        amount: this.remainingAmount(loan)
      }
    });

    dialogRef.afterClosed().subscribe(confirm => {
      if (!confirm) return;

      this.emiService.payAllEmis(loan.loanId).subscribe({
        next: () => {
          loan.EMIs?.forEach(e => (e.paidStatus = true));
          this.snackBar.open('All EMIs paid successfully', 'Close', { duration: 3000 });
        },
        error: () =>
          this.snackBar.open('Payment failed', 'Close', { duration: 3000 })
      });
    });
  }

  // ================= CALCULATIONS =================
  amountPaid(loan: Loan) {
    return loan.EMIs?.filter(e => e.paidStatus)
      .reduce((sum, e) => sum + e.amount, 0) ?? 0;
  }

  remainingAmount(loan: Loan) {
    return loan.EMIs?.filter(e => !e.paidStatus)
      .reduce((sum, e) => sum + e.amount, 0) ?? 0;
  }

  canPayAll(loan: Loan) {
    return loan.status === LoanStatus.Approved &&
           loan.EMIs?.some(e => !e.paidStatus);
  }

  isActiveLoan(loan: Loan) {
    return loan.status === LoanStatus.Approved;
  }

  loanStatusText(status: LoanStatus) {
    return LoanStatus[status];
  }
}

/* ================= PAYMENT DIALOG ================= */

@Component({
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content>
      <p *ngIf="data.amount"><strong>Amount:</strong> ₹{{ data.amount }}</p>
      <p>You will be redirected to secure payment gateway.</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" [mat-dialog-close]="true">
        Pay Now
      </button>
    </mat-dialog-actions>
  `
})
export class PaymentDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
}
@Component({
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>Confirm Payment</h2>
    <mat-dialog-content>
      <p>{{ data.message }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close="false">Cancel</button>
      <button mat-raised-button color="accent" [mat-dialog-close]="true">
        Confirm
      </button>
    </mat-dialog-actions>
  `
})
export class ConfirmPaymentDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: any) {}
}
