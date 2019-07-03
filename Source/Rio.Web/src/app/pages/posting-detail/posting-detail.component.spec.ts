import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostingDetailComponent } from './posting-detail.component';

describe('PostingDetailComponent', () => {
  let component: PostingDetailComponent;
  let fixture: ComponentFixture<PostingDetailComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostingDetailComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostingDetailComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
