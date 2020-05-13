import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ManageWaterAllocationComponent } from './manage-water-allocation.component';

describe('ParcelBulkSetAllocationComponent', () => {
  let component: ManageWaterAllocationComponent;
  let fixture: ComponentFixture<ManageWaterAllocationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ManageWaterAllocationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ManageWaterAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
