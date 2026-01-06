import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportsSummary } from './reports';

describe('Reports', () => {
  let component: ReportsSummary;
  let fixture: ComponentFixture<ReportsSummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ReportsSummary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ReportsSummary);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
