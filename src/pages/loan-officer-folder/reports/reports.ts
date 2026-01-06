import { CommonModule } from '@angular/common';
import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { MatTableModule } from '@angular/material/table';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatGridListModule } from '@angular/material/grid-list'; // ✅ import this

import { ReportsService } from '../../../app/services/reports-service';

export interface LoanStatusReport {
  status: string;
  count: number;
  totalAmount?: number; // optional if your API returns totalAmount
}

export interface ReportsSummaryData {
  totalLoans: number;
  approved: number;
  rejected: number;
  underReview: number;
  closed: number;
  outstandingAmount: number;
}

@Component({
  selector: 'app-reports-summary',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatCardModule,
    MatProgressSpinnerModule,
    MatGridListModule 
  ],
  templateUrl: './reports.html',
  styleUrls: ['./reports.css']
})
export class ReportsSummary implements OnInit {
  displayedColumns: string[] = ['status', 'count'];
  loanStatus: LoanStatusReport[] = [];

  loading = true;
  error: string | null = null;

  summary: ReportsSummaryData | null = null;

  constructor(
    private reportsService: ReportsService,
    private cd: ChangeDetectorRef
  ) {}

  ngOnInit(): void {
    this.reportsService.getLoanStatus().subscribe({
      next: (res) => {
        this.loanStatus = Array.isArray(res) ? res : [];

        // Calculate summary
        this.summary = {
          totalLoans: this.loanStatus.reduce((acc, cur) => acc + cur.count, 0),
          approved: this.getStatusCount('Approved'),
          rejected: this.getStatusCount('Rejected'),
          underReview: this.getStatusCount('Under Review'),
          closed: this.getStatusCount('Closed'),
          outstandingAmount: this.loanStatus.reduce((acc, cur) => acc + (cur.totalAmount || 0), 0)
        };

        this.loading = false;
        this.cd.detectChanges();
      },
      error: (err) => {
        console.error(err);
        this.error = 'Failed to load report data';
        this.loanStatus = [];
        this.loading = false;
        this.cd.detectChanges();
      }
    });
  }

  private getStatusCount(status: string): number {
    const item = this.loanStatus.find((r) => r.status === status);
    return item ? item.count : 0;
  }
}
