import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelDetailComponent } from './parcel-detail.component';

describe('ParcelDetailComponent', () => {
  let component: ParcelDetailComponent;
  let fixture: ComponentFixture<ParcelDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
