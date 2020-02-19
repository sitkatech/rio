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
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';


@Component({
  selector: 'rio-parcel-override-et-data',
  templateUrl: './parcel-override-et-data.component.html',
  styleUrls: ['./parcel-override-et-data.component.scss']
})
export class ParcelOverrideEtDataComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;

  public account: AccountDto;
  public parcels: Array<ParcelDto>;
  public parcelNumbers: string[];
  public waterYears: Array<number>;
  public parcelMonthlyEvaporations: Array<ParcelMonthlyEvapotranspirationDto>;  

  public isEditing: boolean = true;

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
    private accountService: AccountService,
    private alertService: AlertService
  ) {
  }

  ngOnInit() {
    this.parcelMonthlyEvaporations = new Array<ParcelMonthlyEvapotranspirationDto>();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      const waterYear = parseInt(this.route.snapshot.paramMap.get("waterYear"));
      this.waterYearToDisplay = waterYear;
      let accountID = parseInt(this.route.snapshot.paramMap.get("accountID"));
      forkJoin(
        this.accountService.getAccountByID(accountID), this.parcelService.getWaterYears()
      ).subscribe(([account, waterYears]) => {
        this.account = account,
        this.waterYears = waterYears;
        this.updateAnnualData();
      });
    });
  }

  public updateAnnualData() {
    this.parcelService.getParcelsByAccountID(this.account.AccountID, this.waterYearToDisplay).subscribe(parcels=>{
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });
    
    forkJoin(
      this.accountService.getParcelWaterUsageByAccountID(this.account.AccountID, this.waterYearToDisplay)
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
    let sum = this.parcelMonthlyEvaporations.filter(evapData => evapData.WaterMonth == monthNum)
                                            .reduce((sum, current) => sum + (current.OverriddenEvapotranspirationRate || current.EvapotranspirationRate), 0);
    return sum;
  }

  public getParcelEvaporationMonthlyData(parcel : ParcelDto) : Array<ParcelMonthlyEvapotranspirationDto> {
    const parcelMonthlyEvapotranspirations = this.parcelMonthlyEvaporations.filter(x => x.ParcelID === parcel.ParcelID);
    return parcelMonthlyEvapotranspirations;
  }

  public getTotalWaterUsageForParcel(parcel: ParcelDto) : number {
    let parcelEvapotranspirationsDtos = this.getParcelEvaporationMonthlyData(parcel);
    let sum = parcelEvapotranspirationsDtos.reduce((sum, current) => sum + (current.OverriddenEvapotranspirationRate || current.EvapotranspirationRate), 0);
    return sum;
  }

  public getTotalWaterUsageForAccount() : number {
    let sum = this.parcelMonthlyEvaporations.reduce((sum, current) => sum + (current.OverriddenEvapotranspirationRate || current.EvapotranspirationRate), 0);
    return sum;
  }

  public toggleEditMode() : void {
    if (this.isEditing === true) {
      this.cancelOverrideEtChanges();
    }
    else {
      this.isEditing = true;
    }
  }

  public confirmSaveOverrideEtChanges() : void {
    this.accountService.saveParcelMonthlyEvapotranspirationOverrideValues(this.account.AccountID, this.waterYearToDisplay, this.parcelMonthlyEvaporations).subscribe(countSaved=>{
      let saveMessage = `Successfully saved ${countSaved} OpenET usage record${countSaved != 1 ? 's' : ''}`;
      this.alertService.pushAlert(new Alert(saveMessage, AlertContext.Success));
    });
    this.isEditing = false;
  }

  public cancelOverrideEtChanges() : void {
    //Need to bring originals back
    this.updateAnnualData();
    this.isEditing = false;
  }

  public catchExtraSymbols(event : KeyboardEvent) : void {
    if (event.code === "KeyE" || event.code === "Equal" || event.code === "Minus") {
      event.preventDefault();
    }
  }

  public catchPastedSymbols(event : any, parcel : ParcelMonthlyEvapotranspirationDto) : void {
    let val = event.clipboardData.getData('text/plain');
    if (val && (val.includes("+") || val.includes("-") || val.includes("e") || val.includes("E"))) {
      val = val.replace(/\+|\-|e|E/g, '');
      parcel.OverriddenEvapotranspirationRate = val;
      event.preventDefault();
    }
  }
}
