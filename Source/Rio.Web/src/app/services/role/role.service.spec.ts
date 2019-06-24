import { TestBed } from '@angular/core/testing';

import { SystemRoleService } from './role.service';

describe('SystemRoleService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: SystemRoleService = TestBed.get(SystemRoleService);
    expect(service).toBeTruthy();
  });
});
