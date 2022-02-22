import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { DropdownSelectFilterComponent } from './dropdown-select-filter.component';

describe('DropdownSelectFilterComponent', () => {
  let component: DropdownSelectFilterComponent;
  let fixture: ComponentFixture<DropdownSelectFilterComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ DropdownSelectFilterComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DropdownSelectFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
