import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ParcelChangeOwnerComponent } from './parcel-change-owner.component';

describe('ParcelChangeOwnerComponent', () => {
  let component: ParcelChangeOwnerComponent;
  let fixture: ComponentFixture<ParcelChangeOwnerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelChangeOwnerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelChangeOwnerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
