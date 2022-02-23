import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelLedgerCreateComponent } from './parcel-ledger-create.component';

describe('ParcelLedgerCreateComponent', () => {
  let component: ParcelLedgerCreateComponent;
  let fixture: ComponentFixture<ParcelLedgerCreateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelLedgerCreateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelLedgerCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
