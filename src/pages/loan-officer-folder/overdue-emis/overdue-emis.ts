import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { ReportsService } from '../../../app/services/reports-service';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-overdue-emis',
  standalone: true,
  imports: [CommonModule, 
    MatTableModule,
  MatIconModule,
MatCardModule,
],
  templateUrl: './overdue-emis.html'
})
export class OverdueEmisComponent implements OnInit {

  emis: any[] = [];
  cols = ['customer', 'loanType', 'amount', 'dueDate', 'status'];

  constructor(private service: ReportsService) {}

  ngOnInit(): void {
    this.service.getOverdueEmis().subscribe(res => this.emis = res);
  }
}
