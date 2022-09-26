import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMessageListComponent } from './user-message-list.component';

describe('UserMessageListComponent', () => {
  let component: UserMessageListComponent;
  let fixture: ComponentFixture<UserMessageListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserMessageListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserMessageListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
