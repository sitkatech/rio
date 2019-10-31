import { Injectable } from '@angular/core';
import { ApiService } from 'src/app/shared/services';
import { Observable } from 'rxjs';
import { MarketMetricsDto } from '../shared/models/market-metrics-dto';
import { TradeActivityByMonthDto } from '../shared/models/trade-activity-by-month-dto';

@Injectable({
    providedIn: 'root'
})
export class MarketMetricsService {
  constructor(private apiService: ApiService) { }

  public getMarketMetrics(): Observable<MarketMetricsDto> {
      let route = `/market-metrics`;
      return this.apiService.getFromApi(route);
  }

  public getMonthlyTradeActivity(): Observable<Array<TradeActivityByMonthDto>> {
      let route = `/market-metrics/monthly-trade-activity`;
      return this.apiService.getFromApi(route);
  }
}
