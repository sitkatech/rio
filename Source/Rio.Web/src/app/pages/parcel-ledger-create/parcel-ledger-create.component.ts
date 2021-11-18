import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin, OperatorFunction, Observable } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelLedgerCreateDto } from 'src/app/shared/models/parcel/parcel-ledger-create-dto';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { WaterTypeDto } from 'src/app/shared/models/water-type-dto';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeUTCAdapter } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { UserDto } from 'src/app/shared/models';
import { ParcelLedgerDto } from 'src/app/shared/models/parcel/parcel-ledger-dto';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

@Component({
  selector: 'rio-parcel-ledger-create',
  templateUrl: './parcel-ledger-create.component.html',
  styleUrls: ['./parcel-ledger-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeUTCAdapter}]
})
export class ParcelLedgerCreateComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;
  private parcelNumbers: Array<string>;
  public parcel: ParcelDto;
  public parcelLedgers: ParcelLedgerDto[];
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public isLoadingSubmit: boolean = false;
  public allocationID: number = TransactionTypeEnum.Allocation;
  public manualAdjustmentID: number = TransactionTypeEnum.ManualAdjustment;
  public richTextTypeID: number = CustomRichTextType.ParcelLedgerCreate;
  private alertsCountOnLoad: number;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private parcelService: ParcelService,
    private parcelLedgerService: ParcelLedgerService,
    private waterTypeService: WaterTypeService,
  ) { }

  ngOnInit(): void {
    this.model = new ParcelLedgerCreateDto();
    
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
      this.currentUser = currentUser;

      const id = this.route.snapshot.paramMap.get("id");
      if (id) {
        const parcelID = parseInt(id);
        forkJoin(
          this.parcelService.getParcelByParcelID(parcelID),
          this.parcelService.getParcelLedgerEntriesByParcelID(parcelID)
        ).subscribe(
          ([parcel, parcelLedgers]) => {
            this.parcel = parcel instanceof Array
              ? null
              : parcel as ParcelDto;
            this.parcel = parcel;
            this.parcelLedgers = parcelLedgers;
            this.model.ParcelID = parcel.ParcelID;
          }
        );
      } else {
        this.parcel = new ParcelDto;
      }

      forkJoin(
      this.waterTypeService.getWaterTypes(),
      this.parcelService.getAllParcelNumbers()
      ).subscribe(([waterTypes, parcelNumbers]) => {
        this.waterTypes = waterTypes;
        this.parcelNumbers = parcelNumbers;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public isUsageAdjustment(): boolean {
    return this.model.TransactionTypeID != this.allocationID;
  }

  private validateUsageCorrectionDate(): boolean {
    const effectiveDate = this.model.EffectiveDate;
    const currentDate = new Date();
    
    // checks that usage correction is not being applied towards future date
    if (effectiveDate.getFullYear() > currentDate.getFullYear() || effectiveDate.getMonth() > currentDate.getMonth() || effectiveDate.getDate() > currentDate.getDate()) {
      let errorMessage = "Transactions to adjust usage for future dates are not allowed."
      this.alertService.pushAlert(new Alert(errorMessage, AlertContext.Danger));
      return false;
    }
    return true;
  }

  private validateUsageDeposit(): boolean {
    let usageTransactionTypeIDs = [TransactionTypeEnum.MeasuredUsage, TransactionTypeEnum.MeasuredUsageCorrection, TransactionTypeEnum.ManualAdjustment];
    let monthlyUsageSum = Math.abs(this.parcelLedgers
      .filter(x => x.WaterYear == this.model.EffectiveDate.getFullYear() && x.WaterMonth == this.model.EffectiveDate.getMonth() + 1 && usageTransactionTypeIDs.includes(x.TransactionType.TransactionTypeID))
      .reduce((a, b) => {
        return a + b.TransactionAmount;
      }, 0));
      
    // checks that transaction amount doesn't exceed parcel's usage total for effective month
    if (this.model.TransactionAmount > monthlyUsageSum) {
      let errorMessage = "Parcel usage for " + (this.model.EffectiveDate.getMonth() + 1) + "/" + this.model.EffectiveDate.getFullYear() + 
      " is currently " + monthlyUsageSum.toFixed(2) + ". Please update quantity for correction so that usage is not less than 0."
      this.alertService.pushAlert(new Alert(errorMessage, AlertContext.Danger));
      
      return false;
    }
    return true;
  }

  private clearErrorAlerts() {
    if (!this.alertsCountOnLoad) {
      this.alertsCountOnLoad = this.alertService.getAlerts().length;
    }

    this.alertService.removeAlertsSubset(this.alertsCountOnLoad, this.alertService.getAlerts().length - this.alertsCountOnLoad);
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.clearErrorAlerts();

    if (this.isUsageAdjustment) {
      if (!this.validateUsageCorrectionDate() || (!this.model.IsWithdrawal && !this.validateUsageDeposit())) {
        this.isLoadingSubmit = false;
        return;
      }
    }

    this.parcelLedgerService.newTransaction(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        createTransactionForm.reset();
        this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(x => {
          this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
        });
      },
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  search: OperatorFunction<string, readonly string[]> = (text$: Observable<string>) =>
    text$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      map(term => term.length < 2 ? []
        : this.parcelNumbers.filter(v => v.toLowerCase().indexOf(term.toLowerCase()) > -1).slice(0, 10))
    )
}

