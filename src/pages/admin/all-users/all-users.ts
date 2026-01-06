import { Component, signal, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Admin } from '../../../app/services/admin';
import { AdminUser } from '../../../app/models/admin.models';

/* Angular Material */
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-all-users',
  standalone: true,
  imports: [
    CommonModule,
    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatCardModule
  ],
  templateUrl: './all-users.html',
  styleUrls: ['./all-users.css']
})
export class AllUsersComponent implements AfterViewInit {

  loading = signal(true);
  error = signal<string | null>(null);

  displayedColumns: string[] = [
    'userName',
    'email',
    'annualIncome',
    'currentRole',
    'status',
    'actions'
  ];

  dataSource = new MatTableDataSource<AdminUser>([]);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private adminService: Admin) {}

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;

    this.dataSource.sortingDataAccessor = (
      item: AdminUser,
      property: string
    ) => {
      switch (property) {
        case 'userName':
          return item.userName?.toLowerCase() ?? '';

        case 'email':
          return item.email?.toLowerCase() ?? '';

        case 'annualIncome':
          return item.annualIncome ?? 0; // numeric sort

        case 'currentRole':
          return item.currentRole?.toLowerCase() ?? 'pending';

        case 'status':
          return item.isApproved ? 'active' : 'disabled';

        default:
          return '';
      }
    };

    this.loadUsers();
  }

  loadUsers(): void {
    this.loading.set(true);
    this.error.set(null);

    this.adminService.getAllUsers().subscribe({
      next: users => {
        this.dataSource.data = users;
        this.loading.set(false);

        if (this.paginator) {
          this.paginator.firstPage();
        }
      },
      error: () => {
        this.error.set('Failed to load users');
        this.loading.set(false);
      }
    });
  }

  applyFilter(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();

    if (this.paginator) {
      this.paginator.firstPage();
    }
  }

  enableUser(id: string): void {
    this.adminService.enableUser(id).subscribe(() => this.loadUsers());
  }

  disableUser(id: string): void {
    this.adminService.disableUser(id).subscribe(() => this.loadUsers());
  }

  getStatus(user: AdminUser): string {
    return user.isApproved ? 'Active' : 'Disabled';
  }
}
