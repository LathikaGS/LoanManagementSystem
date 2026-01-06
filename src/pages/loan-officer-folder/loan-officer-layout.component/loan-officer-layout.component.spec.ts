import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LoanOfficerLayoutComponent } from './loan-officer-layout.component';

describe('LoanOfficerLayoutComponent', () => {
  let component: LoanOfficerLayoutComponent;
  let fixture: ComponentFixture<LoanOfficerLayoutComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoanOfficerLayoutComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(LoanOfficerLayoutComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
