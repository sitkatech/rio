import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ParcelNumberLinkRendererComponent } from './parcel-number-link-renderer.component';

describe('LinkRendererComponent', () => {
  let component: ParcelNumberLinkRendererComponent;
  let fixture: ComponentFixture<ParcelNumberLinkRendererComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ParcelNumberLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ParcelNumberLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
