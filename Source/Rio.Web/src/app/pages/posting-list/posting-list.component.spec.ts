import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostingListComponent } from './posting-list.component';

describe('PostingListComponent', () => {
  let component: PostingListComponent;
  let fixture: ComponentFixture<PostingListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostingListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostingListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
