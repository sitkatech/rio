import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreateWaterTransactionsComponent } from './create-water-transactions.component';

describe('CreateWaterTransactionsComponent', () => {
  let component: CreateWaterTransactionsComponent;
  let fixture: ComponentFixture<CreateWaterTransactionsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateWaterTransactionsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateWaterTransactionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
