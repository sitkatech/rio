import { TestBed } from '@angular/core/testing';

import { PostingService } from './posting.service';

describe('PostingService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: PostingService = TestBed.get(PostingService);
    expect(service).toBeTruthy();
  });
});
