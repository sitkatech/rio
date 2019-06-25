import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelsHomeComponent } from './parcels-home.component';

describe('ParcelsHomeComponent', () => {
  let component: ParcelsHomeComponent;
  let fixture: ComponentFixture<ParcelsHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelsHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelsHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
