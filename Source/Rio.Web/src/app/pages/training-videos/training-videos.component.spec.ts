import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TrainingVideosComponent } from './training-videos.component';

describe('TrainingVideosComponent', () => {
  let component: TrainingVideosComponent;
  let fixture: ComponentFixture<TrainingVideosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TrainingVideosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TrainingVideosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
