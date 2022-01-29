import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerCreateFromSpreadsheetComponent } from './parcel-ledger-create-from-spreadsheet.component';

describe('ParcelLedgerCreateFromSpreadsheetComponent', () => {
  let component: ParcelLedgerCreateFromSpreadsheetComponent;
  let fixture: ComponentFixture<ParcelLedgerCreateFromSpreadsheetComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerCreateFromSpreadsheetComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerCreateFromSpreadsheetComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
