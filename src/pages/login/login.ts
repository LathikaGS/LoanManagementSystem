import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';

import { AuthService } from '../../app/services/auth';
import { LoginRequest } from '../../app/models/login-request.model';

@Component({
  standalone: true,
  selector: 'app-login',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule
  ],
  templateUrl: './login.html',
  styleUrls: ['./login.css']
})
export class LoginComponent implements OnInit {

  form!: FormGroup;
  error = '';

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  login(): void {
    if (this.form.invalid) return;

    const payload = this.form.value as LoginRequest;

    this.authService.login(payload).subscribe({
      next: res => {
        this.authService.saveAuth(res.token, res.role);
        console.log('Role from backend:', res.role);

        if (res.role === 'Admin') {
          this.router.navigate(['/admin/dashboard']);
        } else if (res.role === 'LoanOfficer') {
          this.router.navigate(['/loan-officer/dashboard']);
        } else {
          this.router.navigate(['customer/dashboard']);
        }
      },
      error: err => {
        this.error = err.error || 'Invalid credentials';
      }
    });
  }
}
