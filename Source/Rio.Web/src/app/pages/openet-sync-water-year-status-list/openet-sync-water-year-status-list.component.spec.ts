import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenetSyncWaterYearStatusListComponent } from './openet-sync-water-year-status-list.component';

describe('OpenetSyncWaterYearStatusListComponent', () => {
  let component: OpenetSyncWaterYearStatusListComponent;
  let fixture: ComponentFixture<OpenetSyncWaterYearStatusListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenetSyncWaterYearStatusListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenetSyncWaterYearStatusListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
