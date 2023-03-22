import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterUseMeasurementComponent } from './measuring-water-use.component';

describe('WaterUseMeasurementComponent', () => {
  let component: WaterUseMeasurementComponent;
  let fixture: ComponentFixture<WaterUseMeasurementComponent>;

  beforeEach(waitForAsync(() => {
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
