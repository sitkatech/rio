import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ParcelLedgerCsvUploadUsageComponent } from './parcel-ledger-csv-upload-usage.component';


describe('ParcelLedgerCsvUploadUsageComponent', () => {
  let component: ParcelLedgerCsvUploadUsageComponent;
  let fixture: ComponentFixture<ParcelLedgerCsvUploadUsageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerCsvUploadUsageComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerCsvUploadUsageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
