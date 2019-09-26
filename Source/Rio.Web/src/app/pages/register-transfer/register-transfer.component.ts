import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { WaterTransferService } from 'src/app/services/water-transfer.service';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { WaterTransferRegistrationDto } from 'src/app/shared/models/water-transfer-registration-dto';
import { WaterTransferTypeEnum } from 'src/app/shared/models/enums/water-transfer-type-enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelPickerComponent } from 'src/app/shared/components/parcel-picker/parcel-picker.component';
import { WaterTransferRegistrationParcelDto } from 'src/app/shared/models/water-transfer-registration-parcel-dto';

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
  public visibleParcelIDs: Array<number> = [];
  public waterTransferType: WaterTransferTypeEnum;
  public haveParcelsBeenIdentified: boolean = false;

  @ViewChild("parcelPicker", { static: false })
  public parcelPicker: ParcelPickerComponent;

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private waterTransferService: WaterTransferService,
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
      const waterTransferID = parseInt(this.route.snapshot.paramMap.get("waterTransferID"));
      if (waterTransferID) {
        this.getData(waterTransferID);
      }
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getData(waterTransferID: number): void {
    forkJoin(
      this.waterTransferService.getWaterTransferFromWaterTransferID(waterTransferID),
      this.parcelService.getParcelsByUserID(this.currentUser.UserID),
      this.waterTransferService.getParcelsForWaterTransferIDAndUserID(waterTransferID, this.currentUser.UserID),
    )
      .subscribe(([waterTransfer, visibleParcels, selectedParcels]) => {
        this.waterTransfer = waterTransfer instanceof Array
          ? null
          : waterTransfer as WaterTransferDto;
        this.waterTransferType = this.waterTransfer.Buyer.UserID === this.currentUser.UserID ? WaterTransferTypeEnum.Buying : WaterTransferTypeEnum.Selling;
        this.isRegisteringTransfer = false;
        this.registerAction = this.waterTransfer.Buyer.UserID === this.currentUser.UserID ? "to" : "from";
        if (!this.canRegister()) {
          this.router.navigateByUrl("/trades/" + waterTransfer.TradeNumber)
        }
        this.visibleParcelIDs = visibleParcels !== undefined ? visibleParcels.map(p => p.ParcelID) : [];
        this.selectedParcels = selectedParcels;
        this.haveParcelsBeenIdentified = this.selectedParcels.length > 0;
      });
  }

  public getTotalPrice(waterTransfer: WaterTransferDto): number {
    return waterTransfer.UnitPrice * waterTransfer.AcreFeetTransferred;
  }

  public canRegister(): boolean {
    return (this.waterTransfer.Buyer.UserID === this.currentUser.UserID && this.waterTransfer.RegisteredByBuyer === false) ||
      (this.waterTransfer.Seller.UserID === this.currentUser.UserID && this.waterTransfer.RegisteredBySeller === false);
  }

  public isBuyerOrSeller(): boolean {
    return this.waterTransfer.Buyer.UserID === this.currentUser.UserID || this.waterTransfer.Seller.UserID === this.currentUser.UserID;
  }

  public cancelRegistration(): void {
    this.haveParcelsBeenIdentified = false;
    this.isRegisteringTransfer = false;
  }

  public submitRegistration(): void {
    this.isLoadingSubmit = true;
    let model = new WaterTransferRegistrationDto();
    model.UserID = this.currentUser.UserID;
    model.WaterTransferTypeID = this.waterTransferType;
    this.waterTransferService.registerTransfer(this.waterTransfer.WaterTransferID, model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/landowner-dashboard")
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
    this.isLoadingSubmit = true;
    const waterTransferRegistrationDto = new WaterTransferRegistrationDto();
    waterTransferRegistrationDto.WaterTransferTypeID = this.waterTransferType;
    let waterTransferRegistrationParcels = this.parcelPicker.selectedParcels.map(p => {
      let waterTransferParcelDto = new WaterTransferRegistrationParcelDto();
      waterTransferParcelDto.ParcelID = p.ParcelID;
      waterTransferParcelDto.AcreFeetTransferred = p.AcreFeetTransferred;
      return waterTransferParcelDto;
    });
    waterTransferRegistrationDto.WaterTransferRegistrationParcels = waterTransferRegistrationParcels;

    this.waterTransferService.selectParcelsForWaterTransferID(this.waterTransfer.WaterTransferID, waterTransferRegistrationDto)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
        this.haveParcelsBeenIdentified = true;
        this.isRegisteringTransfer = true;
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

  public isParcelPickerValid(): boolean {
    if (this.parcelPicker) {
      const totalEqualsTransferAmount = this.parcelPicker.getTotalEntered() === this.waterTransfer.AcreFeetTransferred;
      const parcelsWithAcreFeetTransferered = this.parcelPicker.selectedParcels.filter(x => x.AcreFeetTransferred > 0);
      const everyParcelHasTransferAmountEntered = parcelsWithAcreFeetTransferered.length === this.parcelPicker.selectedParcels.length;
      return totalEqualsTransferAmount && everyParcelHasTransferAmountEntered;
    }
    return false;
  }
}
