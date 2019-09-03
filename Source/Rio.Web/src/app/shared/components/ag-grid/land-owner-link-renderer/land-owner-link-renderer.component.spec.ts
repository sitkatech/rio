import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { LandOwnerLinkRendererComponent } from './land-owner-link-renderer.component';

describe('LandOwnerRendererComponent', () => {
  let component: LandOwnerLinkRendererComponent;
  let fixture: ComponentFixture<LandOwnerLinkRendererComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ LandOwnerLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(LandOwnerLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
