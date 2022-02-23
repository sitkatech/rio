import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ManagedRechargeScenarioComponent } from './managed-recharge-scenario.component';

describe('ManagedRechargeScenarioComponent', () => {
  let component: ManagedRechargeScenarioComponent;
  let fixture: ComponentFixture<ManagedRechargeScenarioComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ManagedRechargeScenarioComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManagedRechargeScenarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
