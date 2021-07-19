import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenetSyncWaterYearMonthStatusListComponent } from './openet-sync-water-year-month-status-list.component';

describe('OpenetSyncWaterYearMonthStatusListComponent', () => {
  let component: OpenetSyncWaterYearMonthStatusListComponent;
  let fixture: ComponentFixture<OpenetSyncWaterYearMonthStatusListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenetSyncWaterYearMonthStatusListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenetSyncWaterYearMonthStatusListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
