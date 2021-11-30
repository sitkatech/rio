import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { Observable, of, forkJoin } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeUTCAdapter } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerCreateDto } from 'src/app/shared/generated/model/parcel-ledger-create-dto';
import { ParcelLedgerDto } from 'src/app/shared/generated/model/parcel-ledger-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { DecimalPipe } from '@angular/common';


@Component({
  selector: 'rio-parcel-ledger-bulk-create',
  templateUrl: './parcel-ledger-bulk-create.component.html',
  styleUrls: ['./parcel-ledger-bulk-create.component.scss']
})
export class ParcelLedgerBulkCreateComponent implements OnInit {
  @ViewChild('parcelSelectGrid') parcelLedgerGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  public currentUser: UserDto;
  public parcels: Array<ParcelDto>;
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public isLoadingSubmit: boolean = false;
  public allocationID: number = TransactionTypeEnum.Allocation;
  public manualAdjustmentID: number = TransactionTypeEnum.ManualAdjustment;
  public richTextTypeID: number = CustomRichTextType.ParcelLedgerCreate;
  private alertsCountOnLoad: number;
  public searchFailed : boolean = false;
  public decimalPipe: DecimalPipe;
  
  public columnDefs: ColDef[];
  public defaultColDef: ColDef;
  public rowData = [];
  

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

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
      })
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private clearErrorAlerts() {
    if (!this.alertsCountOnLoad) {
      this.alertsCountOnLoad = this.alertService.getAlerts().length;
    }

    this.alertService.removeAlertsSubset(this.alertsCountOnLoad, this.alertService.getAlerts().length - this.alertsCountOnLoad);
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    alert("form submitted");
    this.clearErrorAlerts();
    this.isLoadingSubmit = false;

    // this.parcelLedgerService.newTransaction(this.model)
    //   .subscribe(response => {
    //     this.isLoadingSubmit = false;
    //     createTransactionForm.reset();
        
    //     if (this.parcel) {
    //       this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(x => {
    //         this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
    //       });
    //     } else {
    //       this.router.navigateByUrl("/parcels/create-water-transactions").then(x => {
    //         this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
    //       });
    //     }
    //   },
    //     error => {
    //       this.isLoadingSubmit = false;
    //       console.log(error);
    //       this.cdr.detectChanges();
    //     }
    //   );
  }
}
