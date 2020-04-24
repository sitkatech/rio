import { TestBed } from '@angular/core/testing';

import { CustomRichTextService } from './custom-rich-text.service';

describe('CustomRichTextService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CustomRichTextService = TestBed.get(CustomRichTextService);
    expect(service).toBeTruthy();
  });
});
