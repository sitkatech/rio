import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { MultiLinkRendererComponent } from './multi-link-renderer.component';

describe('MultiLinkRendererComponent', () => {
  let component: MultiLinkRendererComponent;
  let fixture: ComponentFixture<MultiLinkRendererComponent>;

  beforeEach(async(() => {
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
