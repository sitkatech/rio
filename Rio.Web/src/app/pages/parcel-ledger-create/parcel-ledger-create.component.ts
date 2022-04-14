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
import { TransactionTypeEnum } from 'src/app/shared/generated/enum/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeUTCAdapter } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { debounceTime, distinctUntilChanged, tap, switchMap, catchError } from 'rxjs/operators';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerCreateDto } from 'src/app/shared/generated/model/parcel-ledger-create-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { NgbDateAdapterFromString } from 'src/app/shared/components/ngb-date-adapter-from-string';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-parcel-ledger-create',
  templateUrl: './parcel-ledger-create.component.html',
  styleUrls: ['./parcel-ledger-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateAdapterFromString}]
})
export class ParcelLedgerCreateComponent implements OnInit {

  
  public currentUser: UserDto;
  public parcelFromRoute: ParcelDto;
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public selectedParcelNumber: string;
  public isLoadingSubmit: boolean = false;
  public richTextTypeID: number = CustomRichTextTypeEnum.ParcelLedgerCreate;
  public searchFailed : boolean = false;
  public transactionTypeEnum = TransactionTypeEnum;

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

    if (!this.includeWaterSupply()) {
      this.model.TransactionTypeID = TransactionTypeEnum.Usage;
    }
    
    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      const id = this.route.snapshot.paramMap.get("id");
      if (id) {
        this.parcelService.getParcelByParcelID(parseInt(id)).subscribe(parcel => {
            this.parcelFromRoute = parcel;
            this.selectedParcelNumber = parcel.ParcelNumber;
          }
        );
      }

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
      });
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
  }

  public includeWaterSupply(): boolean{
    return environment.includeWaterSupply;
  }

  public isUsageAdjustment(): boolean {
    return this.model.TransactionTypeID != TransactionTypeEnum.Supply;
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();

    if (this.selectedParcelNumber && this.selectedParcelNumber != "") {
      this.model.ParcelNumbers = [];
      this.model.ParcelNumbers.push(this.selectedParcelNumber);
    }

    this.parcelLedgerService.newTransaction(this.model)
      .subscribe(() => {
        this.isLoadingSubmit = false;
        createTransactionForm.reset();
        
        if (this.parcelFromRoute) {
          this.router.navigateByUrl("/parcels/" + this.parcelFromRoute.ParcelID).then(x => {
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
          window.scroll(0,0);
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

