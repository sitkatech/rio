import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelChangeOwnerComponent } from './parcel-change-owner.component';

describe('ParcelChangeOwnerComponent', () => {
  let component: ParcelChangeOwnerComponent;
  let fixture: ComponentFixture<ParcelChangeOwnerComponent>;

  beforeEach(async(() => {
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
