import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradesHomeComponent } from './trades-home.component';

describe('TradesHomeComponent', () => {
  let component: TradesHomeComponent;
  let fixture: ComponentFixture<TradesHomeComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradesHomeComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradesHomeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
