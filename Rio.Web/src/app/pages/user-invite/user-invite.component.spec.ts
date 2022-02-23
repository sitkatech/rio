import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UserInviteComponent } from './user-invite.component';

describe('InviteUserDialogComponent', () => {
  let component: UserInviteComponent;
  let fixture: ComponentFixture<UserInviteComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UserInviteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserInviteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
