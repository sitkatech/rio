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
import { ParcelMonthlyEvapotranspirationOverrideDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-override-dto';
import { AccountService } from 'src/app/services/account/account.service';


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
  

  public isEditing: boolean = false;

  public months = ["Jan",
        "Feb",
        "Mar",
        "Apr",
        "May",
        "Jun",
        "Jul",
        "Aug",
        "Sep",
        "Oct",
        "Nov",
        "Dec"];

  constructor(
    private route: ActivatedRoute,
    private userService: UserService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private accountService: AccountService
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
    this.parcelService.getParcelsByAccountID(this.user.UserID, this.waterYearToDisplay).subscribe(parcels=>{
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });
    
    forkJoin(
      this.accountService.getParcelWaterUsageByAccountID(this.user.UserID, this.waterYearToDisplay)
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

  public getTotalWaterUsageForMonth(monthNum : number) : number {
    let sum = 0;
    if (this.parcelMonthlyEvaporations !== null && this.parcelMonthlyEvaporations !== undefined)
    {
      sum = this.parcelMonthlyEvaporations.filter(evapData => evapData.WaterMonth == monthNum)
                                          .reduce((sum, current) => sum + (current.OverriddenEvapotranspirationRate || current.EvapotranspirationRate), 0);
    }
    return sum
  }

  public getParcelEvaporationMonthlyData(parcel : ParcelDto) : Array<ParcelMonthlyEvapotranspirationDto> {
    const parcelMonthlyEvapotranspirations = this.parcelMonthlyEvaporations.filter(x => x.ParcelID === parcel.ParcelID);
    return parcelMonthlyEvapotranspirations;
  }

  public toggleEditMode() : void {
    this.isEditing = this.isEditing === true ? false : true;
  }

  public confirmSaveOverrideEtChanges() : void {
    console.log(this.user.UserID, this.waterYearToDisplay);
    this.accountService.saveParcelMonthlyEvapotranspirationOverrideValues(this.user.UserID, this.waterYearToDisplay, this.parcelMonthlyEvaporations).subscribe(x=>{
      //
    });
    this.isEditing = false;
  }

  public cancelOverrideEtChanges() : void {
    this.parcelMonthlyEvaporations.forEach(x => x.OverriddenEvapotranspirationRate = null);
    this.isEditing = false;
  }
}
