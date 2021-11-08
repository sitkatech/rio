import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { PostingDeleteComponent } from './posting-delete.component';

describe('PostingDeleteComponent', () => {
  let component: PostingDeleteComponent;
  let fixture: ComponentFixture<PostingDeleteComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ PostingDeleteComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostingDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
