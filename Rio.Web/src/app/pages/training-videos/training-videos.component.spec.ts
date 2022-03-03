import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TrainingVideosComponent } from './training-videos.component';

describe('TrainingVideosComponent', () => {
  let component: TrainingVideosComponent;
  let fixture: ComponentFixture<TrainingVideosComponent>;

  beforeEach(waitForAsync(() => {
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
