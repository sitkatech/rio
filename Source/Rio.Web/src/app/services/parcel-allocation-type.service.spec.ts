import { TestBed } from '@angular/core/testing';

import { ParcelAllocationTypeService } from './parcel-allocation-type.service';

describe('ParcelAllocationTypeService', () => {
  let service: ParcelAllocationTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ParcelAllocationTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
