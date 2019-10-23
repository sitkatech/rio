import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelBulkSetAllocationComponent } from './parcel-bulk-set-allocation.component';

describe('ParcelBulkSetAllocationComponent', () => {
  let component: ParcelBulkSetAllocationComponent;
  let fixture: ComponentFixture<ParcelBulkSetAllocationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelBulkSetAllocationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelBulkSetAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
