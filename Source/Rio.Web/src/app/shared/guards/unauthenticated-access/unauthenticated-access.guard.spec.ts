import { TestBed, inject, waitForAsync } from '@angular/core/testing';

import { UnauthenticatedAccessGuard } from './unauthenticated-access.guard';

describe('UnauthenticatedAccessGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UnauthenticatedAccessGuard]
    });
  });

  it('should ...', inject([UnauthenticatedAccessGuard], (guard: UnauthenticatedAccessGuard) => {
    expect(guard).toBeTruthy();
  }));
});
