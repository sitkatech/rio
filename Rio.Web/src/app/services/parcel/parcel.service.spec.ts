import { TestBed } from '@angular/core/testing';

import { ParcelService } from './parcel.service';

describe('ParcelService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: ParcelService = TestBed.get(ParcelService);
    expect(service).toBeTruthy();
  });
});
