import { TestBed } from '@angular/core/testing';

import { MarketMetricsService } from './market-metrics.service';

describe('MarketMetricsService', () => {
  beforeEach(() => TestBed.configureTestingModule({}));

  it('should be created', () => {
    const service: MarketMetricsService = TestBed.get(MarketMetricsService);
    expect(service).toBeTruthy();
  });
});
