import { TestBed } from '@angular/core/testing';

import { WaterTransferService } from './water-transfer.service';

describe('WaterTransferService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: WaterTransferService = TestBed.get(WaterTransferService);
    expect(service).toBeTruthy();
  });
});
