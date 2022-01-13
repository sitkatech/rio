import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { LandownerWaterSupplyChartComponent } from './landowner-water-supply-chart.component';

describe('LandownerWaterSupplyChartComponent', () => {
  let component: LandownerWaterSupplyChartComponent;
  let fixture: ComponentFixture<LandownerWaterSupplyChartComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ LandownerWaterSupplyChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandownerWaterSupplyChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
