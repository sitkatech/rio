import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { OverconsumptionRateEditComponent } from './overconsumption-rate-edit.component';

describe('OverconsumptionRateEditComponent', () => {
  let component: OverconsumptionRateEditComponent;
  let fixture: ComponentFixture<OverconsumptionRateEditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ OverconsumptionRateEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OverconsumptionRateEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
