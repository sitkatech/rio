import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { ClearGridFiltersButtonComponent } from './clear-grid-filters-button.component';

describe('ClearGridFiltersButtonComponent', () => {
  let component: ClearGridFiltersButtonComponent;
  let fixture: ComponentFixture<ClearGridFiltersButtonComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ ClearGridFiltersButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ClearGridFiltersButtonComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
