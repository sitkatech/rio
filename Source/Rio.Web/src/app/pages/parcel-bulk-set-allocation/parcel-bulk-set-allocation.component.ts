import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { DatePipe } from '@angular/common'; 
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'rio-parcel-bulk-set-allocation',
  templateUrl: './parcel-bulk-set-allocation.component.html',
  styleUrls: ['./parcel-bulk-set-allocation.component.scss']
})
export class ParcelBulkSetAllocationComponent implements OnInit, OnDestroy {
  @ViewChild('parcelAllocationHistoryGrid', { static: false }) parcelAllocationHistoryGrid: AgGridAngular;
  @ViewChild('projectWaterAllocation', {static: false}) projectWaterAllocation: any;
  @ViewChild('nativeYieldAllocation', {static: false}) nativeYieldAllocation: any;
  @ViewChild('reconciliationWaterFileUpload', {static: false}) reconciliationWaterFileUpload: any;

  public parcelAllocationHistoryGridColumnDefs: ColDef[];
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: number;
  public waterYears: Array<number>;

  public lastProjectWaterSetDate: string;
  public lastNativeYieldSetDate: string;
  public lastReconciliationFileUploadDate: string;

  public displayProjectWaterError: boolean = false;
  public displayNativeYieldError: boolean = false;
  public displayWaterReconciliationError: boolean = false;

  public displayErrors = [this.displayProjectWaterError, this.displayWaterReconciliationError, this.displayNativeYieldError];
  
  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router, private parcelService: ParcelService, private authenticationService: AuthenticationService, private alertService: AlertService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeParcelAllocationHistoryGrid();
      this.parcelService.getDefaultWaterYearToDisplay().subscribe(defaultYear=>{
        this.waterYearToDisplay = defaultYear;
        this.parcelService.getWaterYears().subscribe(waterYears =>{
          this.waterYears = waterYears;
          this.updateParcelAllocationHistoryGrid();
        }) 
      })     

    });
  }

  private initializeParcelAllocationHistoryGrid(): void {
    let datePipe = this.datePipe;

    this.parcelAllocationHistoryGridColumnDefs = [
      { headerName: 'Date', field: 'Date', valueFormatter: function(params: any) {
        return datePipe.transform(params.data.Date, "M/d/yyyy")
      }, sortable:true, filter: true, width:130},
      { headerName: 'Water Year', field: 'WaterYear', sortable:true, filter: true, width:130},
      { headerName: 'Allocation', field: 'Allocation', sortable:true, filter: true, width:150},
      { headerName: 'Value (ac-ft/ac)', field: 'Value', sortable:true, filter: true},
      { headerName: 'Uploaded Filename', field: 'Filename', sortable:true, filter:true},
      { headerName: 'User', field: 'User', sortable:true, filter: true}
    ];
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public submitBulkAllocation(allocationType: number, allocationValue: string): void {
    this.isLoadingSubmit = true;

    this.turnOffErrors();

    if (allocationValue !== undefined && allocationValue !== null && allocationValue !== "") {
    this.model.AcreFeetAllocated = +allocationValue;
    this.model.ParcelAllocationTypeID = allocationType;
    this.model.WaterYear = this.waterYearToDisplay;

    this.parcelService.bulkSetAnnualAllocations(this.model, this.currentUser.UserID)
      .subscribe(response => {
          this.clearInputs()
          this.isLoadingSubmit = false;
          this.updateParcelAllocationHistoryGrid();
          this.alertService.pushAlert(new Alert("Successfully set Allocations for " + response + " parcels.", AlertContext.Success));
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
    }
    else {
      this.isLoadingSubmit = false;
      this.chooseErrorToDisplay(allocationType);
    }
  }

  public updateParcelAllocationHistoryGrid(): void {
    this.parcelService.getParcelAllocationHistory().subscribe(result => {
      let datePipe = this.datePipe;
      this.lastProjectWaterSetDate = datePipe.transform(result.filter(x => x.Allocation == "Project Water").reduce((x, y) => x.Date > y.Date ? x : y, {Date:null} ).Date, "M/d/yyyy");
      this.lastNativeYieldSetDate = datePipe.transform(result.filter(x => x.Allocation == "Native Yield").reduce((x, y) => x.Date > y.Date ? x : y, {Date:null}).Date, "M/dd/yyyy");
      this.lastReconciliationFileUploadDate = datePipe.transform(result.filter(x => x.Allocation == "Reconciliation").reduce((x, y) => x.Date > y.Date ? x : y, {Date:null}).Date, "M/dd/yyyy");
      this.parcelAllocationHistoryGrid ? this.parcelAllocationHistoryGrid.api.setRowData(result) : null;
    });
  }

  public clearInputs() {
    this.projectWaterAllocation.nativeElement.value = null;
    this.nativeYieldAllocation.nativeElement.value = null;
    this.reconciliationWaterFileUpload.nativeElement.value = null;
  }

  public chooseErrorToDisplay(allocationType: number)
  {
    if (allocationType == 1) {
      this.displayProjectWaterError = true;
    }
    else if (allocationType == 2) {
      this.displayWaterReconciliationError = true;
    }
    else if (allocationType == 3) {
      this.displayNativeYieldError = true;
    }
  }

  public turnOffErrors() {
    this.displayNativeYieldError = false;
    this.displayProjectWaterError = false;
    this.displayWaterReconciliationError = false;
  }
}
