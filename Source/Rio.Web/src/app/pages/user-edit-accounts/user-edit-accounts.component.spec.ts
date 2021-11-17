import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UserEditAccountsComponent } from './user-edit-accounts.component';

describe('UserEditAccountsComponent', () => {
  let component: UserEditAccountsComponent;
  let fixture: ComponentFixture<UserEditAccountsComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UserEditAccountsComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserEditAccountsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
