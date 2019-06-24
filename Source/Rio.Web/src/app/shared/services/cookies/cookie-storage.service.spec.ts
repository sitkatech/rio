import { TestBed } from '@angular/core/testing';

import { CookieStorageService } from './cookie-storage.service';

describe('CookieStorageService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: CookieStorageService = TestBed.get(CookieStorageService);
    expect(service).toBeTruthy();
  });
});
