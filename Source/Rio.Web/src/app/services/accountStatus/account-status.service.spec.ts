import { TestBed } from '@angular/core/testing';

import { AccountStatusService } from './account-status.service';

describe('AccountStatusService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: AccountStatusService = TestBed.get(AccountStatusService);
    expect(service).toBeTruthy();
  });
});
