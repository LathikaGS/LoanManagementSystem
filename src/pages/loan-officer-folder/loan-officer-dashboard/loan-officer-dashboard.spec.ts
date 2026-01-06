import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoanOfficerDashboard } from './loan-officer-dashboard';

describe('LoanOfficerDashboard', () => {
  let component: LoanOfficerDashboard;
  let fixture: ComponentFixture<LoanOfficerDashboard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoanOfficerDashboard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoanOfficerDashboard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
