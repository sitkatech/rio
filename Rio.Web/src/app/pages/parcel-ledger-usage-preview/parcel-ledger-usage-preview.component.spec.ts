import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerUsagePreviewComponent } from './parcel-ledger-usage-preview.component';

describe('ParcelLedgerUsagePreviewComponent', () => {
  let component: ParcelLedgerUsagePreviewComponent;
  let fixture: ComponentFixture<ParcelLedgerUsagePreviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerUsagePreviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerUsagePreviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
