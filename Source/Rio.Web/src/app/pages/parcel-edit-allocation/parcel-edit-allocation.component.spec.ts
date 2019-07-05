import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelEditAllocationComponent } from './parcel-edit-allocation.component';

describe('ParcelEditAllocationComponent', () => {
  let component: ParcelEditAllocationComponent;
  let fixture: ComponentFixture<ParcelEditAllocationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelEditAllocationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelEditAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
