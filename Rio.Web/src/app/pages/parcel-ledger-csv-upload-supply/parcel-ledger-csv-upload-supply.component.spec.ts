import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerCsvUploadSupplyComponent } from './parcel-ledger-csv-upload-supply.component';

describe('ParcelLedgerCsvUploadSupplyComponent', () => {
  let component: ParcelLedgerCsvUploadSupplyComponent;
  let fixture: ComponentFixture<ParcelLedgerCsvUploadSupplyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerCsvUploadSupplyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerCsvUploadSupplyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
