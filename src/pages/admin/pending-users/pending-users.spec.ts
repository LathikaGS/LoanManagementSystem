import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PendingUsersComponent } from './pending-users';

describe('PendingUsers', () => {
  let component: PendingUsersComponent;
  let fixture: ComponentFixture<PendingUsersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PendingUsersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PendingUsersComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
