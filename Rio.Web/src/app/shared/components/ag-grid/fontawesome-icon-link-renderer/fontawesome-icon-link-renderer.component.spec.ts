import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { FontAwesomeIconLinkRendererComponent } from './fontawesome-icon-link-renderer.component';

describe('LandownerDashboardLinkRendererComponent', () => {
  let component: FontAwesomeIconLinkRendererComponent;
  let fixture: ComponentFixture<FontAwesomeIconLinkRendererComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ FontAwesomeIconLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FontAwesomeIconLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
