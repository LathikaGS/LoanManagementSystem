import { Component, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatIconModule } from '@angular/material/icon';
import { ReportsService } from '../../../app/services/reports-service';
import { AuthService } from '../../../app/services/auth';
import { Router } from '@angular/router';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-reports-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatToolbarModule,
    MatIconModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './loan-officer-dashboard.html',
  styleUrls: ['./loan-officer-dashboard.css']
})
export class ReportsDashboardComponent implements OnInit {

  // 🔹 Signals
  summary = signal<any>(null);
  outstanding = signal<number>(0);
  loading = signal<boolean>(true);

  constructor(
    private service: ReportsService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadSummary();
    this.loadOutstanding();
  }

  // ================= LOAD SUMMARY =================
  loadSummary() {
    this.loading.set(true);

    this.service.getSummary().subscribe({
      next: res => {
        this.summary.set(res);
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  // ================= LOAD OUTSTANDING =================
  loadOutstanding() {
    this.service.getOutstanding().subscribe({
      next: res => this.outstanding.set(res.totalOutstanding)
    });
  }

  // ================= LOGOUT =================
  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
