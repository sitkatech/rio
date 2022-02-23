import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LandownerWaterUseChartComponent } from './landowner-water-use-chart.component';

describe('LandownerWaterUseChartComponent', () => {
  let component: LandownerWaterUseChartComponent;
  let fixture: ComponentFixture<LandownerWaterUseChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LandownerWaterUseChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandownerWaterUseChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
