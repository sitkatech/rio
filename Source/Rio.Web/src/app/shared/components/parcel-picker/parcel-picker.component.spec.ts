import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelPickerComponent } from './parcel-picker.component';

describe('ParcelPickerComponent', () => {
  let component: ParcelPickerComponent;
  let fixture: ComponentFixture<ParcelPickerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelPickerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelPickerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
