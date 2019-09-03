import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TradeDateLinkRendererComponent } from './trade-date-link-renderer.component';

describe('TradeDateLinkRendererComponent', () => {
  let component: TradeDateLinkRendererComponent;
  let fixture: ComponentFixture<TradeDateLinkRendererComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TradeDateLinkRendererComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TradeDateLinkRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
