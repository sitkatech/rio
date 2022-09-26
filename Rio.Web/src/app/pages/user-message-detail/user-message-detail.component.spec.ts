import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMessageDetailComponent } from './user-message-detail.component';

describe('UserMessageDetailComponent', () => {
  let component: UserMessageDetailComponent;
  let fixture: ComponentFixture<UserMessageDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserMessageDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserMessageDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
