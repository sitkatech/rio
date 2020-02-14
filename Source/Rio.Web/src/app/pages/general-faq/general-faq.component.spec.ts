import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { GeneralFaqComponent } from './general-faq.component';

describe('GeneralFaqComponent', () => {
  let component: GeneralFaqComponent;
  let fixture: ComponentFixture<GeneralFaqComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ GeneralFaqComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(GeneralFaqComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
