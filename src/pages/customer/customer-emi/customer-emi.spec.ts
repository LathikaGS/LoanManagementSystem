import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomerEmiComponent } from './customer-emi';

describe('CustomerEmi', () => {
  let component: CustomerEmiComponent;
  let fixture: ComponentFixture<CustomerEmiComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CustomerEmiComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(CustomerEmiComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
