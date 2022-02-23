import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { MarketMetricsService } from 'src/app/services/market-metrics.service';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { Color, ColorHelper, ScaleType } from '@swimlane/ngx-charts';
import { MarketMetricsDto } from 'src/app/shared/generated/model/market-metrics-dto';
import { TradeActivityByMonthDto } from 'src/app/shared/generated/model/trade-activity-by-month-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'template-market-metrics-home',
  templateUrl: './market-metrics-home.component.html',
  styleUrls: ['./market-metrics-home.component.scss']
})
export class MarketMetricsHomeComponent implements OnInit, OnDestroy {
  
  private currentUser: UserDto;
  marketMetrics: MarketMetricsDto;
  tradeActivityByMonth: TradeActivityByMonthDto[];
  tradeVolumeByMonthSeries: { name: string; value: number; }[];
  colorScheme: Color = { name: "offerColors", domain: ['#636363', '#ff1100', '#37be23'], selectable: true, group: ScaleType.Ordinal};
  volumeTradedColorScheme: Color = { name: "volumeColors", domain: ['#0f77d2'] , selectable: true, group: ScaleType.Ordinal};
  offerHistorySeries: { name: string; series: { name: string; value: number; }[] }[];
  public lineSeriesColors: ColorHelper;
  
  public readonly priceLineSeries = ["Avg Price", "Min Price", "Max Price"];

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService, 
    private marketMetricsService: MarketMetricsService,
    private currencyPipe: CurrencyPipe,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe
) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      forkJoin(this.marketMetricsService.getMarketMetrics(), this.marketMetricsService.getMonthlyTradeActivity()).subscribe(([marketMetrics, tradeActivityByMonth]) => {
        this.marketMetrics = marketMetrics;
        this.tradeActivityByMonth = tradeActivityByMonth;
        this.tradeVolumeByMonthSeries = tradeActivityByMonth.map(x => {
          return { name: this.datePipe.transform(x.GroupingDate, "MMM yyyy"), value: x.TradeVolume }
        });
        this.offerHistorySeries = [];
        this.offerHistorySeries.push({name: "Avg Price" , series: tradeActivityByMonth.map(x => {
          return { name: this.datePipe.transform(x.GroupingDate, "MMM yyyy"), value: x.AveragePrice }
        })});
        this.offerHistorySeries.push({name: "Min Price" , series: tradeActivityByMonth.map(x => {
          return { name: this.datePipe.transform(x.GroupingDate, "MMM yyyy"), value: x.MinimumPrice }
        })});
        this.offerHistorySeries.push({name: "Max Price" , series: tradeActivityByMonth.map(x => {
          return { name: this.datePipe.transform(x.GroupingDate, "MMM yyyy"), value: x.MaximumPrice }
        })});

        this.lineSeriesColors = new ColorHelper(this.colorScheme, ScaleType.Ordinal, this.priceLineSeries);
      });
    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  private getMostRecentOfferImpl(quantity: number, price: number) : string
  {
    if(quantity)
    {
      return this.decimalPipe.transform(quantity, "1.1-1") + " ac-ft at " + this.currencyPipe.transform(price, "USD") + " per ac-ft";
    }
    return "-";
  }

  public getMostRecentOfferToSell() : string
  {
    return this.getMostRecentOfferImpl(this.marketMetrics.MostRecentOfferToSellQuantity, this.marketMetrics.MostRecentOfferToSellPrice);
  }

  public getMostRecentOfferToBuy() : string
  {
    return this.getMostRecentOfferImpl(this.marketMetrics.MostRecentOfferToBuyQuantity, this.marketMetrics.MostRecentOfferToBuyPrice);
  }

  public getMostRecentRegisteredTrade() : string
  {
    if(this.marketMetrics.MostRecentWaterTransfer)
    {
      return this.decimalPipe.transform(this.marketMetrics.MostRecentWaterTransfer.AcreFeetTransferred, "1.1-1") + " ac-ft at " + this.currencyPipe.transform(this.marketMetrics.MostRecentWaterTransfer.Offer.Price, "USD") + " per ac-ft";
    }
    return "-";
  }
}


