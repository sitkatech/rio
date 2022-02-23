import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerTransactionHistoryComponent } from './parcel-ledger-transaction-history.component';

describe('ParcelLedgerTransactionHistoryComponent', () => {
  let component: ParcelLedgerTransactionHistoryComponent;
  let fixture: ComponentFixture<ParcelLedgerTransactionHistoryComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerTransactionHistoryComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerTransactionHistoryComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
