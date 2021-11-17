import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { WaterTypeEditComponent } from './water-type-edit.component';

describe('WaterTypeEditComponent', () => {
  let component: WaterTypeEditComponent;
  let fixture: ComponentFixture<WaterTypeEditComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterTypeEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterTypeEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
