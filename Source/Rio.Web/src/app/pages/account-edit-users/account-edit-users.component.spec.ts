import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountEditUsersComponent } from './account-edit-users.component';

describe('AccountEditUsersComponent', () => {
  let component: AccountEditUsersComponent;
  let fixture: ComponentFixture<AccountEditUsersComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AccountEditUsersComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AccountEditUsersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
