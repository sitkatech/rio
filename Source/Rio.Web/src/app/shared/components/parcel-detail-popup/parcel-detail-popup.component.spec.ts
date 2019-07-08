import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelDetailPopupComponent } from './parcel-detail-popup.component';

describe('ParcelDetailPopupComponent', () => {
  let component: ParcelDetailPopupComponent;
  let fixture: ComponentFixture<ParcelDetailPopupComponent>;

  beforeEach(async(() => {
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
