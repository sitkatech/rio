import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LandownerDashboardComponent } from './landowner-dashboard.component';

describe('LandownerDashboardComponent', () => {
  let component: LandownerDashboardComponent;
  let fixture: ComponentFixture<LandownerDashboardComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LandownerDashboardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandownerDashboardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
