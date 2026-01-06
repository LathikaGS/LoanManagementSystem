import { Component, OnInit, ViewChild, TemplateRef, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSortModule, Sort } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';

import { LoanOfficerService } from '../../../app/services/loan-officer.service';
import { LoanReviewResponse } from '../../../app/models/loan-review-response.model';
import { LoanStatus } from '../../../app/models/loan-status.model';
import { ApproveLoanDTO } from '../../../app/models/approve-loan.model';

@Component({
  selector: 'app-loan-officer',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTableModule,
    MatButtonModule,
    MatInputModule,
    MatSelectModule,
    MatCardModule,
    MatDialogModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatSnackBarModule
  ],
  templateUrl: './loan-officer.html',
  styleUrls: ['./loan-officer.css']
})
export class LoanOfficerComponent implements OnInit {

  @ViewChild('processDialog') processDialog!: TemplateRef<any>;

  LoanStatus = LoanStatus;

  // ================= SIGNALS =================
  loans = signal<LoanReviewResponse[]>([]);
  searchTerm = signal('');
  filterStatus = signal<LoanStatus | 'All'>('All');
  pageIndex = signal(0);
  pageSize = signal(5);
  sortField = signal<string | null>(null);
  sortDirection = signal<Sort['direction']>('asc');

  selectedLoan!: LoanReviewResponse;
  reviewForm!: FormGroup;

  displayedColumns = ['loanId', 'customerEmail', 'loanType', 'amount', 'tenure', 'status', 'actions'];

  constructor(
    private loanService: LoanOfficerService,
    private dialog: MatDialog,
    private fb: FormBuilder,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadApplications();

    this.reviewForm = this.fb.group({
      status: [LoanStatus.Approved, Validators.required],
      remarks: ['']
    });
  }

  // ================= LOAD LOANS =================
  loadApplications() {
    this.loanService.getApplications().subscribe(res => {
      this.loans.set(res);
    });
  }

  filteredLoans = computed(() => {
    let filtered = this.loans();

    // Search
    const term = this.searchTerm().toLowerCase();
    if (term) {
      filtered = filtered.filter(l =>
        l.customerEmail.toLowerCase().includes(term) ||
        l.loanType.loanName.toLowerCase().includes(term)
      );
    }

    // Filter by status
    if (this.filterStatus() !== 'All') {
      filtered = filtered.filter(l => l.status === this.filterStatus());
    }

    // Sort
    const field = this.sortField();
    const dir = this.sortDirection() === 'asc' ? 1 : -1;
    if (field) {
      filtered = [...filtered].sort((a, b) => {
        const aVal = (a as any)[field] ?? '';
        const bVal = (b as any)[field] ?? '';
        if (typeof aVal === 'string') return aVal.localeCompare(bVal) * dir;
        return (aVal - bVal) * dir;
      });
    }

    return filtered;
  });

  pagedApplications() {
    const start = this.pageIndex() * this.pageSize();
    return this.filteredLoans().slice(start, start + this.pageSize());
  }

  // ================== EVENTS ==================
  searchTermChanged(value: string) {
    this.searchTerm.set(value);
    this.pageIndex.set(0); // reset pagination
  }

  statusFilterChanged(value: LoanStatus | 'All') {
    this.filterStatus.set(value);
    this.pageIndex.set(0); // reset pagination
  }

  onPageChange(event: PageEvent) {
    this.pageIndex.set(event.pageIndex);
    this.pageSize.set(event.pageSize);
  }

  sortData(sort: Sort) {
    this.sortField.set(sort.active);
    this.sortDirection.set(sort.direction || 'asc');
  }

  // ================== DIALOG ==================
  openProcessDialog(loan: LoanReviewResponse) {
    if (loan.status === LoanStatus.Closed) return;
    this.selectedLoan = loan;

    this.reviewForm.reset({
      status: loan.status === LoanStatus.UnderReview ? LoanStatus.UnderReview : LoanStatus.Approved,
      remarks: ''
    });

    this.dialog.open(this.processDialog, { width: '520px' });
  }

  submitReview() {
  const dto = this.reviewForm.value;
  const loanId = this.selectedLoan.loanId;

  //  MARK UNDER REVIEW
  if (dto.status === LoanStatus.UnderReview) {
    this.loanService.markUnderReview(loanId, {
      remarks: dto.remarks
    }).subscribe({
      next: () => {
        this.afterSuccess(LoanStatus.UnderReview);
      },
      error: () => {
        this.snackBar.open('Failed to mark under review', 'Close', { duration: 3000 });
      }
    });
    return;
  }

  this.loanService.reviewLoan(loanId, dto).subscribe({
    next: () => {
      this.afterSuccess(dto.status);
    },
    error: () => {
      this.snackBar.open('Failed to process loan', 'Close', { duration: 3000 });
    }
  });
}

private afterSuccess(status: LoanStatus) {
  this.dialog.closeAll();
  this.loadApplications();

  this.snackBar.open(
    `Loan ${this.loanStatusDisplay[status]} successfully`,
    'Close',
    { duration: 3000 }
  );
}
  // ================== UTILITIES ==================
  loanStatusDisplay: Record<LoanStatus, string> = {
    [LoanStatus.Applied]: 'Applied',
    [LoanStatus.Approved]: 'Approved',
    [LoanStatus.Rejected]: 'Rejected',
    [LoanStatus.UnderReview]: 'Marked Under Review',
    [LoanStatus.Closed]: 'Closed',
  };

  statusText(status: LoanStatus | undefined) {
  if (status === undefined) return '';
  return this.loanStatusDisplay[status];
}


  canProcess(loan: LoanReviewResponse) {
    return loan.status !== LoanStatus.Approved &&
           loan.status !== LoanStatus.Rejected &&
           loan.status !== LoanStatus.Closed;
  }

}
