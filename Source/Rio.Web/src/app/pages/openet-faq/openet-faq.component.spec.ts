import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { OpenetFaqComponent } from './openet-faq.component';

describe('OpenetFaqComponent', () => {
  let component: OpenetFaqComponent;
  let fixture: ComponentFixture<OpenetFaqComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ OpenetFaqComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(OpenetFaqComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
