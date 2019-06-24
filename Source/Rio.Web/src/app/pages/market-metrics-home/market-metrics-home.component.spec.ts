import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MarketMetricsHomeComponent } from './market-metrics-home.component';

describe('MarketMetricsHomeComponent', () => {
  let component: MarketMetricsHomeComponent;
  let fixture: ComponentFixture<MarketMetricsHomeComponent>;

  beforeEach(async(() => {
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
