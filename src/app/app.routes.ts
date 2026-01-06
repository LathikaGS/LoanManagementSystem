import { Routes } from '@angular/router';

// Pages
import { Register } from '../pages/register/register';
import { LoginComponent } from '../pages/login/login';
import { Home } from '../pages/home/home';
import { Dashboard } from '../pages/admin/dashboard/dashboard';
import { AdminLayoutComponent } from '../pages/admin/admin-layout.component/admin-layout.component';
import { PendingUsersComponent } from '../pages/admin/pending-users/pending-users';
import { AllUsersComponent } from '../pages/admin/all-users/all-users';
import { ApplyLoanComponent } from '../pages/customer/apply-loan/apply-loan';
import { CustomerDashboardComponent } from '../pages/customer/dashboard/dashboard';
import { LoanTypesComponent } from '../pages/admin/loan-types/loan-types';
import { LoanOfficerComponent } from '../pages/loan-officer-folder/loan-officer/loan-officer';
import { CustomerEmiComponent } from '../pages/customer/customer-emi/customer-emi';
import { ReportsSummary } from '../pages/loan-officer-folder/reports/reports';
import { ReportsDashboardComponent } from '../pages/loan-officer-folder/loan-officer-dashboard/loan-officer-dashboard';
import { OverdueEmisComponent } from '../pages/loan-officer-folder/overdue-emis/overdue-emis';
import { LoanOfficerLayoutComponent } from '../pages/loan-officer-folder/loan-officer-layout.component/loan-officer-layout.component';
import { CustomerLayoutComponent } from '../pages/customer/customer-layout/customer-layout';
// Guards
import { AuthGuard } from './guards/auth-guard';
import { RoleGuard } from './guards/role-guard';

export const routes: Routes = [
  // Public
  { path: 'register', component: Register },
  { path: 'login', component: LoginComponent },
  { path: 'home', component: Home },
  { path: '', redirectTo: 'home', pathMatch: 'full' },

  // ================= ADMIN =================
  {
  path: 'admin',
  component: AdminLayoutComponent,
  canActivate : [AuthGuard, RoleGuard],
  children: [
    { path: 'dashboard', component: Dashboard },
    { path: 'pending-users', component: PendingUsersComponent },
    { path: 'all-users', component: AllUsersComponent },
    { path: 'loan-types', component: LoanTypesComponent },
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
  ]
},
  // ================= CUSTOMER =================
  {
  path: 'customer',
  component: CustomerLayoutComponent,
  canActivate: [AuthGuard, RoleGuard],
  children: [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    { path: 'dashboard', component: CustomerDashboardComponent },
    { path: 'apply-loan', component: ApplyLoanComponent },
    { path: 'customer-emi', component: CustomerEmiComponent}
  ]
},

  {
    path: 'loan-officer',
    component: LoanOfficerLayoutComponent,
    canActivate : [AuthGuard, RoleGuard],
    children: [
      { path: 'dashboard', component: ReportsDashboardComponent },
      { path: 'reports', component: ReportsSummary },
      { path: 'overdue', component: OverdueEmisComponent },
      { path: 'approve-loan', component: LoanOfficerComponent},
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
    ]
  },
  { path: '**', redirectTo: 'home' }
];
