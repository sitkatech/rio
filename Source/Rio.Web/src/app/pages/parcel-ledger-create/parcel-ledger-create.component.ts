import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
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
  public allocationID: number = TransactionTypeEnum.Allocation;
  public manualAdjustmentID: number = TransactionTypeEnum.ManualAdjustment;
  public richTextTypeID: number = CustomRichTextType.ParcelLedgerCreate;
  private alertsCountOnLoad: number;
  public searchFailed : boolean = false;

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
        this.parcelService.getParcelByParcelID(parseInt(id)).subscribe(parcel => {
            this.parcel = parcel;
            this.model.ParcelNumber = parcel.ParcelNumber;
          }
        );
      }

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
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

  private clearErrorAlerts() {
    if (!this.alertsCountOnLoad) {
      this.alertsCountOnLoad = this.alertService.getAlerts().length;
    }

    this.alertService.removeAlertsSubset(this.alertsCountOnLoad, this.alertService.getAlerts().length - this.alertsCountOnLoad);
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.clearErrorAlerts();

    this.parcelLedgerService.newTransaction(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        createTransactionForm.reset();
        
        if (this.parcel) {
          this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(x => {
            this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
          });
        } else {
          this.router.navigateByUrl("/parcels/create-water-transactions").then(x => {
            this.alertService.pushAlert(new Alert("Your transaction was successfully created.", AlertContext.Success));
          });
        }
      },
        error => {
          this.isLoadingSubmit = false;
          console.log(error);
          this.cdr.detectChanges();
        }
      );
  }

  search = (text$: Observable<string>) => {
    return text$.pipe(      
        debounceTime(200), 
        distinctUntilChanged(),
        tap(() => this.searchFailed = false),
        switchMap(searchText => searchText.length > 2 ? this.parcelService.searchParcelNumber(searchText) : ([])), 
        catchError(() => {
          this.searchFailed = true;
          return of([]);
        })     
      );                 
  }
}

