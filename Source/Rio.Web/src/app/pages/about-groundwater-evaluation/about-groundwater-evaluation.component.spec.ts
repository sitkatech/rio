import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AboutGroundwaterEvaluationComponent } from './about-groundwater-evaluation.component';

describe('AboutGroundwaterEvaluationComponent', () => {
  let component: AboutGroundwaterEvaluationComponent;
  let fixture: ComponentFixture<AboutGroundwaterEvaluationComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AboutGroundwaterEvaluationComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AboutGroundwaterEvaluationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
