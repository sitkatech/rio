import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { forkJoin } from 'rxjs';
import { MarketMetricsService } from 'src/app/services/market-metrics.service';
import { MarketMetricsDto } from 'src/app/shared/models/market-metrics-dto';
import { CurrencyPipe, DecimalPipe, DatePipe } from '@angular/common';
import { TradeActivityByMonthDto } from 'src/app/shared/models/trade-activity-by-month-dto';

@Component({
  selector: 'template-market-metrics-home',
  templateUrl: './market-metrics-home.component.html',
  styleUrls: ['./market-metrics-home.component.scss']
})
export class MarketMetricsHomeComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  marketMetrics: MarketMetricsDto;
  tradeActivityByMonth: TradeActivityByMonthDto[];
  tradeActivityByMonthSeries: { name: string; value: number; }[];
  colorScheme: { domain: string[]; };

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService, 
    private marketMetricsService: MarketMetricsService,
    private currencyPipe: CurrencyPipe,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe
) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      forkJoin(this.marketMetricsService.getMarketMetrics(), this.marketMetricsService.getMonthlyTradeActivity()).subscribe(([marketMetrics, tradeActivityByMonth]) => {
        this.marketMetrics = marketMetrics;
        this.tradeActivityByMonth = tradeActivityByMonth;
        console.log(tradeActivityByMonth);
        this.tradeActivityByMonthSeries = tradeActivityByMonth.map(x => {
          return { name: this.datePipe.transform(x.GroupingDate, "MMMM yyyy"), value: x.TradeVolume }
        });

        this.colorScheme = {
          domain: ['#636363', '#ff1100', '#0400d6'] 
        };
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getMostRecentOfferImpl(quantity: number, price: number) : string
  {
    if(quantity)
    {
      return this.decimalPipe.transform(quantity, "1.0-0") + " ac-ft at " + this.currencyPipe.transform(price, "USD") + " per ac-ft";
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
      return this.marketMetrics.MostRecentWaterTransfer.AcreFeetTransferred + " ac-ft at $" + this.marketMetrics.MostRecentWaterTransfer.UnitPrice + " per ac-ft";
    }
    return "-";
  }
}


