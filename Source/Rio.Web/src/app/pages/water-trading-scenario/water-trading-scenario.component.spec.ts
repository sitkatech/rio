import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WaterTradingScenarioComponent } from './water-trading-scenario.component';

describe('WaterTradingScenarioComponent', () => {
  let component: WaterTradingScenarioComponent;
  let fixture: ComponentFixture<WaterTradingScenarioComponent>;

  beforeEach(async(() => {
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
