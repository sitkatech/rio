import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MarketMetricsHomeComponent } from './market-metrics-home.component';

describe('MarketMetricsHomeComponent', () => {
  let component: MarketMetricsHomeComponent;
  let fixture: ComponentFixture<MarketMetricsHomeComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ MarketMetricsHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MarketMetricsHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
