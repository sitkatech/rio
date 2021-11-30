import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerBulkCreateComponent } from './parcel-ledger-bulk-create.component';

describe('ParcelLedgerBulkCreateComponent', () => {
  let component: ParcelLedgerBulkCreateComponent;
  let fixture: ComponentFixture<ParcelLedgerBulkCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ParcelLedgerBulkCreateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerBulkCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
