import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandownerWaterAllocationChartComponent } from './landowner-water-allocation-chart.component';

describe('LandownerWaterAllocationChartComponent', () => {
  let component: LandownerWaterAllocationChartComponent;
  let fixture: ComponentFixture<LandownerWaterAllocationChartComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandownerWaterAllocationChartComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandownerWaterAllocationChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
