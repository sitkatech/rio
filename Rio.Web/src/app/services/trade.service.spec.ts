import { TestBed } from '@angular/core/testing';

import { TradeService } from './trade.service';

describe('TradeService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: TradeService = TestBed.get(TradeService);
    expect(service).toBeTruthy();
  });
});
