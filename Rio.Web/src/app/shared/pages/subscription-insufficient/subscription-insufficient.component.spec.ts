import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { SubscriptionInsufficientComponent } from './subscription-insufficient.component';

describe('SubscriptionInsufficientComponent', () => {
  let component: SubscriptionInsufficientComponent;
  let fixture: ComponentFixture<SubscriptionInsufficientComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ SubscriptionInsufficientComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SubscriptionInsufficientComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
