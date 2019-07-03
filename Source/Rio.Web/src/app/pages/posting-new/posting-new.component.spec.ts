import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostingNewComponent } from './posting-new.component';

describe('PostingNewComponent', () => {
  let component: PostingNewComponent;
  let fixture: ComponentFixture<PostingNewComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostingNewComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostingNewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
