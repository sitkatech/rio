import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TradeDetailComponent } from './trade-detail.component';

describe('TradeDetailComponent', () => {
  let component: TradeDetailComponent;
  let fixture: ComponentFixture<TradeDetailComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
