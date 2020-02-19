import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WaterUseMeasurementComponent } from './water-use-measurement.component';

describe('WaterUseMeasurementComponent', () => {
  let component: WaterUseMeasurementComponent;
  let fixture: ComponentFixture<WaterUseMeasurementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterUseMeasurementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterUseMeasurementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});