import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AccountReconciliationComponent } from './account-reconciliation.component';

describe('AccountReconciliationComponent', () => {
  let component: AccountReconciliationComponent;
  let fixture: ComponentFixture<AccountReconciliationComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountReconciliationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountReconciliationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
