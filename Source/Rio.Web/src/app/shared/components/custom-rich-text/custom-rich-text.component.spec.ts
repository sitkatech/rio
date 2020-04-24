import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { CustomRichTextComponent } from './custom-rich-text.component';

describe('CustomRichTextComponent', () => {
  let component: CustomRichTextComponent;
  let fixture: ComponentFixture<CustomRichTextComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ CustomRichTextComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CustomRichTextComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
