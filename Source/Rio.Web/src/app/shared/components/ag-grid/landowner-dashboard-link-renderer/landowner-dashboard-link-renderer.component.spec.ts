import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandownerDashboardLinkRendererComponent } from './landowner-dashboard-link-renderer.component';

describe('LandownerDashboardLinkRendererComponent', () => {
  let component: LandownerDashboardLinkRendererComponent;
  let fixture: ComponentFixture<LandownerDashboardLinkRendererComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandownerDashboardLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandownerDashboardLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
