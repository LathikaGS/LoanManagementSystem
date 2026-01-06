import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoanOfficer } from './loan-officer';

describe('LoanOfficer', () => {
  let component: LoanOfficer;
  let fixture: ComponentFixture<LoanOfficer>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoanOfficer]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoanOfficer);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
