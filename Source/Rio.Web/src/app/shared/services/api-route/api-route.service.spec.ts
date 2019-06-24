import { TestBed } from '@angular/core/testing';

import { ApiRouteService } from './api-route.service';

describe('ApiRouteService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ApiRouteService = TestBed.get(ApiRouteService);
    expect(service).toBeTruthy();
  });
});
