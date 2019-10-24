import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { forkJoin } from 'rxjs';
import { MarketMetricsService } from 'src/app/services/market-metrics.service';
import { MarketMetricsDto } from 'src/app/shared/models/market-metrics-dto';

@Component({
  selector: 'template-market-metrics-home',
  templateUrl: './market-metrics-home.component.html',
  styleUrls: ['./market-metrics-home.component.scss']
})
export class MarketMetricsHomeComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  marketMetrics: MarketMetricsDto;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService, private marketMetricsService: MarketMetricsService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      forkJoin(this.marketMetricsService.getMarketMetrics()).subscribe(([marketMetrics]) => {
        this.marketMetrics = marketMetrics;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getMostRecentOfferToSell() : string
  {
    if(this.marketMetrics.MostRecentOfferToSell)
    {
      return this.marketMetrics.MostRecentOfferToSell.Quantity + " ac-ft at $" + this.marketMetrics.MostRecentOfferToSell.Price + " per ac-ft";
    }
    return "-";
  }

  public getMostRecentOfferToBuy() : string
  {
    if(this.marketMetrics.MostRecentOfferToBuy)
    {
      return this.marketMetrics.MostRecentOfferToBuy.Quantity + " ac-ft at $" + this.marketMetrics.MostRecentOfferToSell.Price + " per ac-ft";
    }
    return "-";
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


