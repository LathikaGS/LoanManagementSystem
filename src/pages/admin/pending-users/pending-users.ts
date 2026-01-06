import { Component, AfterViewInit, ViewChild, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { Admin } from '../../../app/services/admin';
import { AdminUser } from '../../../app/models/admin.models';

/* Angular Material */
import { MatTableDataSource, MatTableModule } from '@angular/material/table';
import { MatPaginator, MatPaginatorModule } from '@angular/material/paginator';
import { MatSort, MatSortModule } from '@angular/material/sort';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-pending-users',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,

    MatTableModule,
    MatPaginatorModule,
    MatSortModule,
    MatButtonModule,
    MatInputModule,
    MatCardModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './pending-users.html',
  styleUrls: ['./pending-users.css']
})
export class PendingUsersComponent implements AfterViewInit {

  displayedColumns = ['email', 'requestedRole', 'actions'];
  dataSource = new MatTableDataSource<AdminUser>();

  loading = signal(true);
  error = signal<string | null>(null);

  @ViewChild(MatPaginator) paginator!: MatPaginator;
  @ViewChild(MatSort) sort!: MatSort;

  constructor(private adminService: Admin) {}

  ngAfterViewInit(): void {
    this.dataSource.paginator = this.paginator;
    this.dataSource.sort = this.sort;
    this.loadPendingUsers();
  }

  loadPendingUsers() {
    this.loading.set(true);
    this.error.set(null);

    this.adminService.getPendingUsers().subscribe({
      next: users => {
        this.dataSource.data = users;
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load pending users');
        this.loading.set(false);
      }
    });
  }

  approveUser(userId: string) {
    this.adminService.approveUser(userId).subscribe({
      next: () => this.loadPendingUsers(),
      error: () => this.error.set('Failed to approve user')
    });
  }

  rejectUser(userId: string) {
    this.adminService.rejectUser(userId).subscribe({
      next: () => this.loadPendingUsers(),
      error: () => this.error.set('Failed to reject user')
    });
  }

  applyFilter(event: Event) {
    const value = (event.target as HTMLInputElement).value;
    this.dataSource.filter = value.trim().toLowerCase();
  }
}
