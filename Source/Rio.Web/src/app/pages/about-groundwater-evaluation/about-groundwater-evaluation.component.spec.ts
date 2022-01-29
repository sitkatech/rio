import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AboutGroundwaterEvaluationComponent } from './about-groundwater-evaluation.component';

describe('AboutGroundwaterEvaluationComponent', () => {
  let component: AboutGroundwaterEvaluationComponent;
  let fixture: ComponentFixture<AboutGroundwaterEvaluationComponent>;

  beforeEach(waitForAsync(() => {
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
