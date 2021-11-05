import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { MultiLinkRendererComponent } from './multi-link-renderer.component';

describe('MultiLinkRendererComponent', () => {
  let component: MultiLinkRendererComponent;
  let fixture: ComponentFixture<MultiLinkRendererComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ MultiLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MultiLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
