import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { forkJoin } from 'rxjs';
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
import { UserSimpleDto } from 'src/app/shared/models/user/user-simple-dto';

@Component({
  selector: 'rio-parcel-ledger-create',
  templateUrl: './parcel-ledger-create.component.html',
  styleUrls: ['./parcel-ledger-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeUTCAdapter}]
})
export class ParcelLedgerCreateComponent implements OnInit {

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;
  public parcel: ParcelDto;
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public isLoadingSubmit: boolean = false;
  public isWithdrawal: boolean = true;
  public allocationID = TransactionTypeEnum.Allocation;
  public manualAdjustmentID = TransactionTypeEnum.ManualAdjustment;
  public comment: string = '';
  public richTextTypeID = CustomRichTextType.ParcelLedgerCreate;

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
      console.log(currentUser);
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      forkJoin(
        this.parcelService.getParcelByParcelID(id),
        this.waterTypeService.getWaterTypes()
      ).subscribe(
        ([parcel, waterTypes]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcel = parcel;
          this.model.ParcelID = parcel.ParcelID;
          this.waterTypes = waterTypes;
        }
      );
    });
  }

  public isUsageAdjustment(): boolean {
    return this.model.TransactionTypeID != this.allocationID;
  }

  private updateTransactionAmountSign(): void {
    if (this.isWithdrawal) {
      this.model.TransactionAmount *= -1;
    }
  }

  private isTransactionFormValid(): boolean {
    let formValid = true;

    // const year = this.model.EffectiveDate.getFullYear();
    // if (year > 9999 || year < 1000) {
    //   this.alertService.pushAlert(new Alert("Error: Please enter a 4-digit year.", AlertContext.Danger));
    //   formValid = false;
    // }

    if (this.model.TransactionAmount < 0) {
      const errorMessage = "Error: Please enter a positive quantity. " + 
      ( this.isWithdrawal ?
        "A withdrawal will apply a negative correction by default." : 
        "To apply a negative correction, select withdrawal."
      );
      this.alertService.pushAlert(new Alert(errorMessage, AlertContext.Danger));
      formValid = false;
    }

    return formValid;
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    
    if (!this.isTransactionFormValid()) {
      this.isLoadingSubmit = false;
      return;
    }

    this.updateTransactionAmountSign();
    
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
}

