import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelUpdateLayerComponent } from './parcel-update-layer.component';

describe('ParcelUpdateLayerComponent', () => {
  let component: ParcelUpdateLayerComponent;
  let fixture: ComponentFixture<ParcelUpdateLayerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelUpdateLayerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelUpdateLayerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
