import { TestBed } from '@angular/core/testing';

import { WaterTypeService } from './water-type.service';

describe('WaterTypeService', () => {
  let service: WaterTypeService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(WaterTypeService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
