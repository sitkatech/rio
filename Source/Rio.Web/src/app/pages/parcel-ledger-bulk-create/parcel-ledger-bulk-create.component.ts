import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerCreateDto } from 'src/app/shared/generated/model/parcel-ledger-create-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi } from 'ag-grid-community';
import { DecimalPipe } from '@angular/common';


@Component({
  selector: 'rio-parcel-ledger-bulk-create',
  templateUrl: './parcel-ledger-bulk-create.component.html',
  styleUrls: ['./parcel-ledger-bulk-create.component.scss']
})
export class ParcelLedgerBulkCreateComponent implements OnInit {
  @ViewChild('parcelSelectGrid') parcelSelectGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private gridApi;
  private columnApi;
  public currentUser: UserDto;
  public parcels: Array<ParcelDto>;
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public isLoadingSubmit: boolean = false;

  public richTextTypeID: number = CustomRichTextType.ParcelLedgerBulkCreate;
  private alertsCountOnLoad: number;
  public searchFailed : boolean = false;
  
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
    private waterTypeService: WaterTypeService,
    private decimalPipe: DecimalPipe
  ) { }

  ngOnInit(): void {
    this.model = new ParcelLedgerCreateDto();
    
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
      this.currentUser = currentUser;

      forkJoin(
        this.waterTypeService.getWaterTypes(),
        this.parcelService.getParcelAllocationAndUsagesByYear(new Date().getFullYear())
      ).subscribe(([waterTypes, parcelAllocationAndUsagesByYear]) => {
        this.waterTypes = waterTypes;
        this.rowData = parcelAllocationAndUsagesByYear;

        this.insertWaterTypeColDefs();
      });
    });

    this.initializeParcelSelectGrid();
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private onGridReady(params) {
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
  }

  private initializeParcelSelectGrid() {
    let _decimalPipe = this.decimalPipe;
    this.columnDefs = [
      { filter: false, sortable: false, cellRenderer: function(params) { return '<input type="checkbox">'}},
      { headerName: 'APN', field: 'ParcelNumber' },
      {
        headerName: 'Area (acres)', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
        valueGetter: function(params: any) { return parseFloat(_decimalPipe.transform(params.data.ParcelAreaInAcres, '1.1-1')); }
      },
      { 
        headerName: 'Account', field: 'LandOwner.AccountDisplayName', cellRenderer: function(params: any) {
          return '<a href="/accounts/'+ params.data.LandOwner.AccountID +'">' + params.value + '</a>';
        }
      },
      { 
        headerName: 'Total Allocation', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
        valueGetter: function(params: any) { return parseFloat(_decimalPipe.transform(params.data.Allocation, '1.1-1')); }
      }
    ];

    this.defaultColDef = {
      sortable: true, filter: true
    }
  }

  private insertWaterTypeColDefs() {
    let _decimalPipe = this.decimalPipe;
    let colDefsWithWaterTypes = this.columnDefs;

    this.waterTypes.forEach(waterType => {
      colDefsWithWaterTypes.push(
        {
          headerName: waterType.WaterTypeName, field: 'Allocations[waterType.WaterTypeID]', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
          valueGetter: function (params) { return !params.data.Allocations[waterType.WaterTypeID] ? 0.0 : 
            parseFloat(_decimalPipe.transform(params.data.Allocations[waterType.WaterTypeID], "1.1-1"))
          },
        }
      );
      this.gridApi.setColumnDefs(colDefsWithWaterTypes);
    });
    this.parcelSelectGrid.api.setRowData(this.rowData);
    this.columnApi.autoSizeAllColumns();
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
