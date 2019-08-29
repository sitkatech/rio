import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ComboChartTestComponent } from './combo-chart-test.component';

describe('ComboChartTestComponent', () => {
  let component: ComboChartTestComponent;
  let fixture: ComponentFixture<ComboChartTestComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ComboChartTestComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ComboChartTestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
