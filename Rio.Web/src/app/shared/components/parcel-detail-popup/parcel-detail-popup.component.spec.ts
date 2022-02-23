import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelDetailPopupComponent } from './parcel-detail-popup.component';

describe('ParcelDetailPopupComponent', () => {
  let component: ParcelDetailPopupComponent;
  let fixture: ComponentFixture<ParcelDetailPopupComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelDetailPopupComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelDetailPopupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
