import { TestBed, inject, waitForAsync } from '@angular/core/testing';

import { AcknowledgedDisclaimerGuard } from './acknowledged-disclaimer-guard';

describe('AcknowledgedDisclaimerGuardGuard', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AcknowledgedDisclaimerGuard]
    });
  });

  it('should ...', inject([AcknowledgedDisclaimerGuard], (guard: AcknowledgedDisclaimerGuard) => {
    expect(guard).toBeTruthy();
  }));
});
