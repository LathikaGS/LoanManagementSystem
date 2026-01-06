import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatGridListModule } from '@angular/material/grid-list';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';

import { EmiService } from '../../../app/services/emi.service';
import { Loan } from '../../../app/models/loan.model';
import { LoanStatus } from '../../../app/models/loan-status.model';

interface LoanStatusCount {
  status: LoanStatus;
  count: number;
}

@Component({
  selector: 'app-customer-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatGridListModule,
    MatProgressSpinnerModule,
    MatIconModule
  ],
  templateUrl: './dashboard.html',
  styleUrls: ['./dashboard.css']
})
export class CustomerDashboardComponent {
  loans = signal<Loan[]>([]);
  loading = signal(true);

  constructor(private emiService: EmiService) {
    this.fetchLoans();
  }

  // ================= FETCH LOANS =================
  fetchLoans() {
    this.loading.set(true);
    this.emiService.getLoans().subscribe({
      next: (loans) => {
        this.loans.set(loans);
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  // ================= COMPUTE STATUS COUNTS =================
  loanStatusCounts = computed<LoanStatusCount[]>(() => {
    const map = new Map<LoanStatus, number>();
    for (const loan of this.loans()) {
      map.set(loan.status, (map.get(loan.status) ?? 0) + 1);
    }
    return Array.from(map.entries())
      .sort(([a], [b]) => this.statusOrder(a) - this.statusOrder(b))
      .map(([status, count]) => ({ status, count }));
  });

  // Helper for sorting
  private statusOrder(status: LoanStatus) {
    return [
      LoanStatus.Applied,
      LoanStatus.UnderReview,
      LoanStatus.Approved,
      LoanStatus.Rejected,
      LoanStatus.Closed
    ].indexOf(status);
  }

  // ================= UI HELPERS =================
  statusText(status: LoanStatus) {
    return LoanStatus[status];
  }

  statusIcon(status: LoanStatus) {
    switch (status) {
      case LoanStatus.Applied: return 'assignment';
      case LoanStatus.UnderReview: return 'hourglass_top';
      case LoanStatus.Approved: return 'check_circle';
      case LoanStatus.Rejected: return 'cancel';
      case LoanStatus.Closed: return 'done_all';
      default: return 'help';
    }
  }

  statusCardColor(status: LoanStatus) {
    switch (status) {
      case LoanStatus.Applied: return '#90caf9';
      case LoanStatus.UnderReview: return '#42a5f5';
      case LoanStatus.Approved: return '#66bb6a';
      case LoanStatus.Rejected: return '#ef5350';
      case LoanStatus.Closed: return '#8d6e63';
      default: return '#b0bec5';
    }
  }
}
