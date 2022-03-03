import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { UnauthenticatedComponent } from './unauthenticated.component';

describe('UnauthenticatedComponent', () => {
  let component: UnauthenticatedComponent;
  let fixture: ComponentFixture<UnauthenticatedComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ UnauthenticatedComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UnauthenticatedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
