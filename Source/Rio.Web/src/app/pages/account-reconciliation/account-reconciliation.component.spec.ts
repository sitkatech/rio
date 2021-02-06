import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountReconciliationComponent } from './account-reconciliation.component';

describe('AccountReconciliationComponent', () => {
  let component: AccountReconciliationComponent;
  let fixture: ComponentFixture<AccountReconciliationComponent>;

  beforeEach(async(() => {
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
