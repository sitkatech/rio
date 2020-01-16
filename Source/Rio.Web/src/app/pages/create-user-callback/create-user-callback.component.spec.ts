import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CreateUserCallbackComponent } from './create-user-callback.component';

describe('CreateUserCallbackComponent', () => {
  let component: CreateUserCallbackComponent;
  let fixture: ComponentFixture<CreateUserCallbackComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CreateUserCallbackComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CreateUserCallbackComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
