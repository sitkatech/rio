import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { MarketMetricsDto } from '../shared/models/market-metrics-dto';

@Injectable({
    providedIn: 'root'
})
export class MarketMetricsService {
  constructor(private apiService: ApiService) { }

  public getMarketMetrics(): Observable<MarketMetricsDto> {
      let route = `/market-metrics`;
      return this.apiService.getFromApi(route);
  }
}
