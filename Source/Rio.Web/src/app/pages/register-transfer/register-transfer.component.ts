import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { WaterTransferService } from 'src/app/services/water-transfer.service';
import { WaterTransferTypeEnum } from 'src/app/shared/models/enums/water-transfer-type-enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelPickerComponent } from 'src/app/shared/components/parcel-picker/parcel-picker.component';
import { TradeService } from 'src/app/services/trade.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTransferDto } from 'src/app/shared/generated/model/water-transfer-dto';
import { WaterTransferRegistrationDto } from 'src/app/shared/generated/model/water-transfer-registration-dto';
import { WaterTransferRegistrationParcelDto } from 'src/app/shared/generated/model/water-transfer-registration-parcel-dto';


@Component({
  selector: 'rio-register-transfer',
  templateUrl: './register-transfer.component.html',
  styleUrls: ['./register-transfer.component.scss']
})
export class RegisterTransferComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public waterTransfer: WaterTransferDto;
  public isRegisteringTransfer: boolean = false;
  public registerAction: string;
  public isLoadingSubmit: boolean = false;
  public selectedParcels: Array<WaterTransferRegistrationParcelDto> = [];
  public visibleParcels: Array<ParcelDto> = [];
  public waterTransferType: WaterTransferTypeEnum;
  public haveParcelsBeenIdentified: boolean = false;
  public confirmedFullNameForCancelation: string;
  public confirmedFullNameForRegistration: string;

  @ViewChild("parcelPicker")
  public parcelPicker: ParcelPickerComponent;
  public accountID: number;
  currentUserAccounts: AccountSimpleDto[];

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private waterTransferService: WaterTransferService,
    private tradeService: TradeService,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      const waterTransferID = parseInt(this.route.snapshot.paramMap.get("waterTransferID"));
      this.accountID = parseInt(this.route.snapshot.paramMap.get("accountID"));

      if (!waterTransferID || !this.accountID || this.currentUserAccounts?.filter(x => x.AccountID === this.accountID).length == 0) {
        this.router.navigate(["/"]).then(() => {
          this.alertService.pushNotFoundUnauthorizedAlert();
        });
      }

      this.getData(waterTransferID);
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getData(waterTransferID: number): void {
    forkJoin([
      this.waterTransferService.getWaterTransferFromWaterTransferID(waterTransferID),
      this.parcelService.getParcelsByAccountID(this.accountID, new Date().getFullYear() ),
      this.waterTransferService.getParcelsForWaterTransferIDAndUserID(waterTransferID, this.currentUser.UserID),
    ])
      .subscribe(([waterTransfer, visibleParcels, selectedParcels]) => {
        this.waterTransfer = waterTransfer instanceof Array
          ? null
          : waterTransfer as WaterTransferDto;

        if (this.waterTransfer.BuyerRegistration.Account.AccountID != this.accountID && this.waterTransfer.SellerRegistration.Account.AccountID != this.accountID) {
          this.router.navigate(["/"]).then(() => {
            this.alertService.pushNotFoundUnauthorizedAlert();
          });
        }

        this.waterTransferType = this.waterTransfer.BuyerRegistration.Account.AccountID === this.accountID ? WaterTransferTypeEnum.Buying : WaterTransferTypeEnum.Selling;
        this.isRegisteringTransfer = false;
        this.registerAction = this.waterTransfer.BuyerRegistration.Account.AccountID === this.accountID ? "to" : "from";
        if (!this.canRegister()) {
          this.router.navigateByUrl(`/trades/${waterTransfer.TradeNumber}/${this.accountID}`)
        }
        this.visibleParcels = visibleParcels !== undefined ? visibleParcels : [];
        this.selectedParcels = selectedParcels;
        this.haveParcelsBeenIdentified = this.selectedParcels.length > 0;
      });
  }

  public getTotalPrice(waterTransfer: WaterTransferDto): number {
    return waterTransfer.UnitPrice * waterTransfer.AcreFeetTransferred;
  }

  public canRegister(): boolean {
    return !this.isCanceled() &&    
    (this.waterTransfer.BuyerRegistration.Account.AccountID === this.accountID && !this.waterTransfer.BuyerRegistration.IsRegistered) ||
      (this.waterTransfer.SellerRegistration.Account.AccountID === this.accountID && !this.waterTransfer.SellerRegistration.IsRegistered);
  }

  private isCanceled() {
    return this.waterTransfer.BuyerRegistration.IsCanceled || this.waterTransfer.SellerRegistration.IsCanceled;
  }

  public isBuyerOrSeller(): boolean {
    return this.waterTransfer.BuyerRegistration.Account.AccountID === this.accountID || this.waterTransfer.SellerRegistration.Account.AccountID === this.accountID;
  }

  public isFullNameConfirmedForCancelation(): boolean {
    if(this.confirmedFullNameForCancelation)
    {
      return this.confirmedFullNameForCancelation.toLowerCase() === this.currentUser.FullName.toLowerCase();
    }
    return false;
  }

  public isFullNameConfirmedForRegistration(): boolean {
    if(this.confirmedFullNameForRegistration)
    {
      return this.confirmedFullNameForRegistration.toLowerCase() === this.currentUser.FullName.toLowerCase();
    }
    return false;
  }

  public cancelRegistration(): void {
    this.haveParcelsBeenIdentified = false;
    this.isRegisteringTransfer = false;
    this.confirmedFullNameForRegistration = null;
  }

  public submitRegistration(): void {
    this.isLoadingSubmit = true;
    let model = new WaterTransferRegistrationDto();
    model.UserID = this.currentUser.UserID;
    model.WaterTransferTypeID = this.waterTransferType;
    this.waterTransferService.registerTransfer(this.waterTransfer.WaterTransferID, model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl(`/trades/${this.waterTransfer.TradeNumber}/${this.accountID}`)
          .then(() => {
            this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
          });
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public onSubmitParcels(): void {
    this.parcelPicker.isLoadingSubmit = true;
    const waterTransferRegistrationDto = new WaterTransferRegistrationDto();
    waterTransferRegistrationDto.WaterTransferTypeID = this.waterTransferType;
    waterTransferRegistrationDto.UserID = this.currentUser.UserID;
    let waterTransferRegistrationParcels = this.parcelPicker.selectedParcels.map(p => {
      let waterTransferParcelDto = new WaterTransferRegistrationParcelDto();
      waterTransferParcelDto.ParcelID = p.ParcelID;
      waterTransferParcelDto.AcreFeetTransferred = Math.round(p.AcreFeetTransferred);
      return waterTransferParcelDto;
    });
    waterTransferRegistrationDto.WaterTransferRegistrationParcels = waterTransferRegistrationParcels;
    this.waterTransferService.selectParcelsForWaterTransferID(this.waterTransfer.WaterTransferID, waterTransferRegistrationDto)
      .subscribe(response => {
        this.parcelPicker.isLoadingSubmit = false;
        this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
        this.haveParcelsBeenIdentified = true;
        this.isRegisteringTransfer = true;
      }
        ,
        error => {
          new Alert("There was a problem submitting the parcels for this registration.",
            AlertContext.Danger);
          this.parcelPicker.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public cancelTrade(): void {
    this.isLoadingSubmit = true;
    let model = new WaterTransferRegistrationDto();
    model.UserID = this.currentUser.UserID;
    model.WaterTransferTypeID = this.waterTransferType;
    this.waterTransferService.cancelTrade(this.waterTransfer.WaterTransferID, model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl(`/trades/${this.waterTransfer.TradeNumber}/${this.accountID}`)
          .then(() => {
            this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
          });
      }
        ,
        error => {
          new Alert("There was a problem submitting the parcels for this registration.",
            AlertContext.Danger);
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public canCancelTrade(): boolean {
    return this.isBuyerOrSeller()
      && !this.waterTransfer.BuyerRegistration.IsRegistered
      && !this.waterTransfer.SellerRegistration.IsRegistered;
  }
}
