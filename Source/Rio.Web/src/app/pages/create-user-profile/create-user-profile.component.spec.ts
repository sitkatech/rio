import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { CreateUserProfileComponent } from './create-user-profile.component';

describe('SignUpComponent', () => {
  let component: CreateUserProfileComponent;
  let fixture: ComponentFixture<CreateUserProfileComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateUserProfileComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateUserProfileComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
