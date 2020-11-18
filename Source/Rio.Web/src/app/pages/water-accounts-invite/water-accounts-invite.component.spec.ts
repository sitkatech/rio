import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { WaterAccountsInviteComponent } from './water-accounts-invite.component';

describe('WaterAccountsInviteComponent', () => {
  let component: WaterAccountsInviteComponent;
  let fixture: ComponentFixture<WaterAccountsInviteComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ WaterAccountsInviteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(WaterAccountsInviteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
