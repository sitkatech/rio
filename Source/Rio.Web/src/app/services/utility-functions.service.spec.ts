import { TestBed } from '@angular/core/testing';

import { UtilityFunctionsService } from './utility-functions.service';

describe('UtilityFunctionsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: UtilityFunctionsService = TestBed.get(UtilityFunctionsService);
    expect(service).toBeTruthy();
  });
});
