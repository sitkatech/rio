import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SetWaterAllocationComponent } from './set-water-allocation.component';

describe('ParcelBulkSetAllocationComponent', () => {
  let component: SetWaterAllocationComponent;
  let fixture: ComponentFixture<SetWaterAllocationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SetWaterAllocationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SetWaterAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
