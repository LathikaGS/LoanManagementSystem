import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { Router } from '@angular/router';
@Component({
  selector: 'app-home',
  imports: [CommonModule, 
    MatButtonModule, 
    MatCardModule,
    MatIconModule],
  templateUrl: './home.html',
  styleUrl: './home.css',
  standalone: true,
})
export class Home {
    constructor(private router: Router) {}

    goToLogin(){
      this.router.navigate(['/login']);
    }

    goToRegister(){
      this.router.navigate(['/register']);
    }
}
