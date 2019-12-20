import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { UserService } from 'src/app/services/user/user.service';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { TradeService } from 'src/app/services/trade.service';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { TradeWithMostRecentOfferDto } from 'src/app/shared/models/offer/trade-with-most-recent-offer-dto';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { PostingService } from 'src/app/services/posting.service';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { WaterAllocationOverviewDto, WaterUsageDto } from 'src/app/shared/models/water-usage-dto';
import { MultiSeriesEntry, SeriesEntry } from "src/app/shared/models/series-entry";
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationTypeEnum } from 'src/app/shared/models/enums/parcel-allocation-type-enum';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';


@Component({
  selector: 'rio-parcel-override-et-data',
  templateUrl: './parcel-override-et-data.component.html',
  styleUrls: ['./parcel-override-et-data.component.scss']
})
export class ParcelOverrideEtDataComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;

  public user: UserDto;
  public parcels: Array<ParcelDto>;
  public parcelNumbers: string[];
  public waterYears: Array<number>;
  public parcelMonthlyEvaporations: Array<ParcelMonthlyEvapotranspirationDto>;

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      const waterYear = parseInt(this.route.snapshot.paramMap.get("waterYear"));
      this.waterYearToDisplay = waterYear;
      let userID = parseInt(this.route.snapshot.paramMap.get("userID"));
      forkJoin(
        this.userService.getUserFromUserID(userID), this.parcelService.getWaterYears()
      ).subscribe(([user, waterYears]) => {

        this.user = user;
        this.waterYears = waterYears;
        this.updateAnnualData();
      });
    });
  }

  public updateAnnualData() {
    this.parcelService.getParcelsByUserID(this.user.UserID, this.waterYearToDisplay).subscribe(parcels=>{
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });
    
    forkJoin(
      this.userService.getParcelWaterUsageByUserID(this.user.UserID, this.waterYearToDisplay)
    ).subscribe(([ parcelMonthlyEvaporations]) =>{
      this.parcelMonthlyEvaporations = parcelMonthlyEvaporations;
    })
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return Array.from(new Set(this.parcels.map((item: any) => item.ParcelID)));
  }
}