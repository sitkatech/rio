import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PostingEditComponent } from './posting-edit.component';

describe('PostingEditComponent', () => {
  let component: PostingEditComponent;
  let fixture: ComponentFixture<PostingEditComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PostingEditComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PostingEditComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
