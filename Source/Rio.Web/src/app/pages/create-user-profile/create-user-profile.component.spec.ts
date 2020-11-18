import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUserProfileComponent } from './create-user-profile.component';

describe('SignUpComponent', () => {
  let component: CreateUserProfileComponent;
  let fixture: ComponentFixture<CreateUserProfileComponent>;

  beforeEach(async(() => {
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
