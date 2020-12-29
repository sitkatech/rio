import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelMonthlyEvapotranspirationDto } from 'src/app/shared/models/parcel/parcel-monthly-evapotranspiration-dto';
import { AccountService } from 'src/app/services/account/account.service';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";
import { WaterYearService } from 'src/app/services/water-year.service';


@Component({
  selector: 'rio-parcel-override-et-data',
  templateUrl: './parcel-override-et-data.component.html',
  styleUrls: ['./parcel-override-et-data.component.scss']
})
export class ParcelOverrideEtDataComponent implements OnInit, OnDestroy {
  public waterYearToDisplay: WaterYearDto;
  public currentMonth: number;
  public currentYear: number;
  public currentUser: UserDto;
  private watchUserChangeSubscription: any;

  public account: AccountDto;
  public parcels: Array<ParcelDto>;
  public parcelNumbers: string[];
  public waterYears: Array<WaterYearDto>;
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
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
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
      let accountID = parseInt(this.route.snapshot.paramMap.get("accountID"));
      forkJoin([
        this.accountService.getAccountByID(accountID), 
        this.waterYearService.getWaterYears(),
        this.waterYearService.getDefaultWaterYearToDisplay()
      ]).subscribe(([account, waterYears, defaultWaterYear]) => {
        this.account = account,
        this.waterYears = waterYears;
        let waterYearAvailable = this.waterYears.some(x => x.Year == waterYear);
        if (!waterYearAvailable) {
          this.alertService.pushAlert(new Alert(`Water Year:${waterYear} not available for editing. Please select a different year from the dropdown provided`, AlertContext.Info));
        }
        this.waterYearToDisplay = waterYearAvailable ? this.waterYears.filter(x => x.Year == waterYear)[0] : defaultWaterYear;
        this.updateAnnualData();
      });
    });
  }

  public updateAnnualData() {
    this.parcelService.getParcelsByAccountID(this.account.AccountID, this.waterYearToDisplay.Year).subscribe(parcels=>{
      this.parcels = parcels;
      this.parcelNumbers = Array.from(new Set(parcels.map(x => x.ParcelNumber)));
    });
    
    forkJoin(
      this.accountService.getParcelWaterUsageByAccountID(this.account.AccountID, this.waterYearToDisplay.Year)
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
    this.accountService.saveParcelMonthlyEvapotranspirationOverrideValues(this.account.AccountID, this.waterYearToDisplay.Year, this.parcelMonthlyEvaporations).subscribe(countSaved=>{
      let saveMessage = `Successfully saved ${countSaved} OpenET usage record${countSaved != 1 ? 's' : ''}`;
      this.alertService.pushAlert(new Alert(saveMessage, AlertContext.Success));
      this.updateAnnualData();
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

  public determineContentsOfCell(parcelEvapotranspirationData : ParcelMonthlyEvapotranspirationDto) : any {
    if ((!this.isEditing || this.checkIfDateColumnIsAFutureDate(parcelEvapotranspirationData.WaterMonth)) && parcelEvapotranspirationData.OverriddenEvapotranspirationRate !== null)
    {
      return parcelEvapotranspirationData.OverriddenEvapotranspirationRate.toFixed(1);
    }
    else if (parcelEvapotranspirationData.IsEmpty)
    {
      return "-";
    }
    else 
    {
      return parcelEvapotranspirationData.EvapotranspirationRate.toFixed(1);
    }
  }

  public checkIfDateColumnIsAFutureDate(month: number) : boolean {
      var date = new Date();
      return (this.waterYearToDisplay.Year > date.getFullYear() || (this.waterYearToDisplay.Year == date.getFullYear() && month > date.getMonth() + 1))
  }
}
