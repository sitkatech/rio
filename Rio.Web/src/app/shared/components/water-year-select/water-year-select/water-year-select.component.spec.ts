import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterYearSelectComponent } from './water-year-select.component';

describe('WaterYearSelectComponent', () => {
  let component: WaterYearSelectComponent;
  let fixture: ComponentFixture<WaterYearSelectComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterYearSelectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterYearSelectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
