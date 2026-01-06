import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AllUsersComponent } from './all-users';

describe('AllUsers', () => {
  let component: AllUsersComponent;
  let fixture: ComponentFixture<AllUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AllUsersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AllUsersComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
