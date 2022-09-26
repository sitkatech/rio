import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserMessageNewComponent } from './user-message-new.component';

describe('UserMessageNewComponent', () => {
  let component: UserMessageNewComponent;
  let fixture: ComponentFixture<UserMessageNewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserMessageNewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserMessageNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
