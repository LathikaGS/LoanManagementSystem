import { Component, OnInit, Inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSelectModule } from '@angular/material/select';
import { MatDialog, MatDialogModule, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

import { AuthService } from '../../app/services/auth';
import { RegisterRequest } from '../../app/models/register-request.model';

interface DialogData {
  title: string;
  message: string;
}

/* ---------------- DIALOG COMPONENT ---------------- */
@Component({
  selector: 'app-register-dialog',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule],
  template: `
    <h2 mat-dialog-title>{{ data.title }}</h2>
    <mat-dialog-content>
      <p style="white-space: pre-line;">{{ data.message }}</p>
    </mat-dialog-content>
    <mat-dialog-actions align="end">
      <button mat-button mat-dialog-close>OK</button>
    </mat-dialog-actions>
  `
})
export class RegisterDialogComponent {
  constructor(@Inject(MAT_DIALOG_DATA) public data: DialogData) {}
}

/* ---------------- REGISTER COMPONENT ---------------- */
@Component({
  standalone: true,
  selector: 'app-register',
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatSelectModule,
    MatDialogModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './register.html',
  styleUrls: ['./register.css']
})
export class Register implements OnInit {

  form!: FormGroup;
  loading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.form = this.fb.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
      role: ['', Validators.required],
      annualIncome: [
        null,
        [Validators.required, Validators.min(1)]
      ]
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.showDialog(
        'Validation Error',
        'Please fill all fields correctly.'
      );
      return;
    }

    const payload: RegisterRequest = this.form.value;
    this.loading = true;

    this.authService.register(payload).subscribe({
      next: () => {
        this.loading = false;
        this.showDialog(
          'Success',
          'Registration successful.\nWaiting for admin approval.'
        )
          .afterClosed()
          .subscribe(() => this.router.navigate(['/login']));
      },
      error: err => {
        this.loading = false;

        let message = 'Registration failed.';

        if (err.error?.errors) {
          message = Object.values(err.error.errors).flat().join('\n');
        } else if (err.error?.message) {
          message = err.error.message;
        } else if (typeof err.error === 'string') {
          message = err.error;
        }

        this.showDialog('Error', message);
      }
    });
  }

  private showDialog(title: string, message: string) {
    return this.dialog.open(RegisterDialogComponent, {
      data: { title, message }
    });
  }
}
