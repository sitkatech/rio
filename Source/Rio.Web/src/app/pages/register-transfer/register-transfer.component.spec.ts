import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { RegisterTransferComponent as RegisterTransferComponent } from './register-transfer.component';

describe('ConfirmTransferComponent', () => {
  let component: RegisterTransferComponent;
  let fixture: ComponentFixture<RegisterTransferComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ RegisterTransferComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RegisterTransferComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
