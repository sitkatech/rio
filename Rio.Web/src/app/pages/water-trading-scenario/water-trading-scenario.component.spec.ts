import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterTradingScenarioComponent } from './water-trading-scenario.component';

describe('WaterTradingScenarioComponent', () => {
  let component: WaterTradingScenarioComponent;
  let fixture: ComponentFixture<WaterTradingScenarioComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterTradingScenarioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterTradingScenarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
