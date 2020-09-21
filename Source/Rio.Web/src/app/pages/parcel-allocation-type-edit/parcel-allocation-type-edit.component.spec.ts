import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelAllocationTypeEditComponent } from './parcel-allocation-type-edit.component';

describe('ParcelAllocationTypeEditComponent', () => {
  let component: ParcelAllocationTypeEditComponent;
  let fixture: ComponentFixture<ParcelAllocationTypeEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelAllocationTypeEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelAllocationTypeEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
