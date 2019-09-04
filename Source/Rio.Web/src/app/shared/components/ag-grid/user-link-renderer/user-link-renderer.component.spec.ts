import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UserLinkRendererComponent } from './user-link-renderer.component';

describe('LandOwnerRendererComponent', () => {
  let component: UserLinkRendererComponent;
  let fixture: ComponentFixture<UserLinkRendererComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UserLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UserLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
