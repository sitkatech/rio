import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountNewComponent } from './account-new.component';

describe('AccountNewComponent', () => {
  let component: AccountNewComponent;
  let fixture: ComponentFixture<AccountNewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountNewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
