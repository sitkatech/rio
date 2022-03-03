import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TagBulkParcelsComponent } from './tag-bulk-parcels.component';

describe('TagBulkParcelsComponent', () => {
  let component: TagBulkParcelsComponent;
  let fixture: ComponentFixture<TagBulkParcelsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TagBulkParcelsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TagBulkParcelsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
