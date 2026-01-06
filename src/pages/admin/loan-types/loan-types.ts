import { CommonModule } from '@angular/common';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';

import { LoanType } from '../../../app/models/loan-type.model';
import { LoanTypeService } from '../../../app/services/loan-type.service';

import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Inject } from '@angular/core';

/* ============================ ADD / EDIT DIALOG ============================ */
@Component({
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, MatButtonModule, MatInputModule],
  template: `
    <h2 mat-dialog-title>{{ data.mode === 'add' ? 'Add Loan Type' : 'Edit Loan Type' }}</h2>

    <mat-dialog-content>
      <mat-form-field appearance="outline" class="full">
        <mat-label>Loan Name</mat-label>
        <input matInput [(ngModel)]="data.form.loanName" />
      </mat-form-field>

      <mat-form-field appearance="outline" class="full">
        <mat-label>Rate of Interest (%)</mat-label>
        <input matInput type="number" [(ngModel)]="data.form.roi" />
      </mat-form-field>

      <mat-form-field appearance="outline" class="full">
        <mat-label>Max Tenure (months)</mat-label>
        <input matInput type="number" [(ngModel)]="data.form.maxTenure" />
      </mat-form-field>
    </mat-dialog-content>

    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>Cancel</button>
      <button mat-raised-button color="primary" (click)="save()">
        {{ data.mode === 'add' ? 'Add' : 'Update' }}
      </button>
    </mat-dialog-actions>
  `,
  styles: [`.full { width: 100%; margin-bottom: 12px; }`]
})
export class LoanTypeDialogComponent {
  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: { form: LoanType; mode: 'add' | 'edit' }
  ) {}

  save() {
    if (!this.data.form.loanName || this.data.form.roi <= 0 || this.data.form.maxTenure <= 0) return;
    (window as any).dialogRef.close(this.data.form);
  }
}

/* ============================ MAIN COMPONENT ============================ */
@Component({
  selector: 'app-loan-types',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatTableModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    MatPaginatorModule,
    MatSortModule,
    MatIconModule,
    MatDialogModule
  ],
  templateUrl: './loan-types.html',
  styleUrls: ['./loan-types.css']
})
export class LoanTypesComponent implements OnInit {
  dataSource = new MatTableDataSource<LoanType>();
  displayedColumns = ['id', 'name', 'roi', 'tenure', 'actions'];

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private loanTypeService: LoanTypeService, private dialog: MatDialog) {}

  ngOnInit(): void {
    this.loadLoanTypes();
  }

  ngAfterViewInit() {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.dataSource.sortingDataAccessor = (item, property) => {
      if (property === 'name') return item.loanName.toLowerCase();
      return (item as any)[property];
    };
  }

  loadLoanTypes() {
    this.loanTypeService.getAll().subscribe({
      next: data => (this.dataSource.data = data)
    });
  }

  addLoanType() {
    const ref = this.dialog.open(LoanTypeDialogComponent, {
      width: '400px',
      data: { mode: 'add', form: { loanName: '', roi: 0, maxTenure: 0 } }
    });
    (window as any).dialogRef = ref;

    ref.afterClosed().subscribe(result => {
      if (!result) return;
      this.loanTypeService.create(result).subscribe(() => this.loadLoanTypes());
    });
  }

  editLoanType(item: LoanType) {
    const ref = this.dialog.open(LoanTypeDialogComponent, {
      width: '400px',
      data: { mode: 'edit', form: { ...item } }
    });
    (window as any).dialogRef = ref;

    ref.afterClosed().subscribe(result => {
      if (!result) return;
      this.loanTypeService.update(item.loanTypeId!, result).subscribe(() => this.loadLoanTypes());
    });
  }

  deleteLoanType(id: number) {
    this.loanTypeService.delete(id).subscribe(() => this.loadLoanTypes());
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }
}
