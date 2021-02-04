import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelListInactiveComponent } from './parcel-list-inactive.component';

describe('ParcelListInactiveComponent', () => {
  let component: ParcelListInactiveComponent;
  let fixture: ComponentFixture<ParcelListInactiveComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelListInactiveComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelListInactiveComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
