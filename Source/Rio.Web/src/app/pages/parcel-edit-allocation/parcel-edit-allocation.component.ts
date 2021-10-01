import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelLedgerDto } from 'src/app/shared/models/parcel/parcel-ledger-dto';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";
import { TransactionTypeService } from 'src/app/services/transaction-type.service';
import { TransactionTypeDto } from 'src/app/shared/models/transaction-type-dto';

@Component({
  selector: 'rio-parcel-edit-allocation',
  templateUrl: './parcel-edit-allocation.component.html',
  styleUrls: ['./parcel-edit-allocation.component.scss']
})
export class ParcelEditAllocationComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;

  public waterYears: Array<WaterYearDto>;
  public parcel: ParcelDto;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public isLoadingSubmit: boolean = false;
  transactionTypes: TransactionTypeDto[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private alertService: AlertService,
    private authenticationService: AuthenticationService,
    private transactionTypeService: TransactionTypeService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(() => {
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelAllocations(id),
          this.waterYearService.getWaterYears(),
          this.transactionTypeService.getAllocationTypes()
        ).subscribe(
          ([parcel, parcelLedgers, waterYears, transactionTypes]) => {
            this.parcel = parcel instanceof Array
              ? null
              : parcel as ParcelDto;
            this.parcel = parcel;
            this.parcelLedgers = parcelLedgers;
            this.waterYears = waterYears;
            this.transactionTypes = transactionTypes;
          }
        );
      }
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }
  
  public getAllocationForYearByType(transactionType: TransactionTypeDto, year: number): number {
    var parcelLedger = this.parcelLedgers.find(x => x.WaterYear === year && x.TransactionTypeID === transactionType.TransactionTypeID);
    return parcelLedger ? parcelLedger.TransactionAmount : null;
  }

  public updateAllocationModel(transactionType: TransactionTypeDto, year: number, $event: Event): void{
    const newValue = Number((<HTMLInputElement>$event.target).value);
    let updatedParcelLedger = this.parcelLedgers.find(x=>x.TransactionTypeID === transactionType.TransactionTypeID && x.WaterYear === year );
    if (updatedParcelLedger !== null && updatedParcelLedger !== undefined) {
      updatedParcelLedger.TransactionAmount = newValue;
    } else{
      const newParcelLedger = new ParcelLedgerDto({TransactionTypeID : transactionType.TransactionTypeID, transactionDate: new Date(year, 1, 1), ParcelID: this.parcel.ParcelID, TransactionAmount: newValue});
      this.parcelLedgers.push(newParcelLedger);
    }

    this.cdr.detectChanges;
  }

  public onSubmit(editAllocationForm: HTMLFormElement): void {
         
    this.parcelService.mergeParcelAllocations(this.parcel.ParcelID, this.parcelLedgers)
      .subscribe(() => {
        this.isLoadingSubmit = false;
        editAllocationForm.reset();
        this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(() => {
          this.alertService.pushAlert(new Alert(`The allocations for ${this.parcel.ParcelNumber} were successfully updated.`, AlertContext.Success));
        });
      }
        ,
        () => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }
}
