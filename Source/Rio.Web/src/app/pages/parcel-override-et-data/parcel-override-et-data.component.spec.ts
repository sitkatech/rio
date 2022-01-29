import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelOverrideEtDataComponent } from './parcel-override-et-data.component';

describe('ParcelOverrideEtDataComponent', () => {
  let component: ParcelOverrideEtDataComponent;
  let fixture: ComponentFixture<ParcelOverrideEtDataComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelOverrideEtDataComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelOverrideEtDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
