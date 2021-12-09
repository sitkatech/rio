import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckboxRendererComponent } from './checkbox-renderer.component';

describe('CheckboxRendererComponent', () => {
  let component: CheckboxRendererComponent;
  let fixture: ComponentFixture<CheckboxRendererComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckboxRendererComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckboxRendererComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
