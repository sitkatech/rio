import { TestBed } from '@angular/core/testing';

import { PostingTypeService } from './posting-type.service';

describe('PostingTypeService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PostingTypeService = TestBed.get(PostingTypeService);
    expect(service).toBeTruthy();
  });
});
