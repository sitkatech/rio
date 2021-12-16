import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { ParcelLedgerCreateDto } from 'src/app/shared/generated/model/parcel-ledger-create-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridOptions, RowSelectedEvent } from 'ag-grid-community';
import { DecimalPipe } from '@angular/common';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ParcelAllocationAndUsageDto } from 'src/app/shared/generated/model/parcel-allocation-and-usage-dto';

@Component({
  selector: 'rio-parcel-ledger-bulk-create',
  templateUrl: './parcel-ledger-bulk-create.component.html',
  styleUrls: ['./parcel-ledger-bulk-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeAdapter}]

})
export class ParcelLedgerBulkCreateComponent implements OnInit {
  @ViewChild('parcelSelectGrid') parcelSelectGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private gridApi;
  private columnApi;
  public currentUser: UserDto;
  public waterTypes: WaterTypeDto[];
  public model: ParcelLedgerCreateDto;
  public noParcelsSelected: boolean = true;
  public isLoadingSubmit: boolean = false;

  public richTextTypeID: number = CustomRichTextType.ParcelLedgerBulkCreate;
  private alertsCountOnLoad: number;
  public searchFailed : boolean = false;
  
  public parcelAllocationAndUsagesByYear: ParcelAllocationAndUsageDto[];
  public columnDefs: ColDef[];
  public defaultColDef: ColDef;
  public gridOptions: GridOptions;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private parcelService: ParcelService,
    private parcelLedgerService: ParcelLedgerService,
    private waterTypeService: WaterTypeService,
    private decimalPipe: DecimalPipe
  ) { }

  ngOnInit(): void {
    this.model = new ParcelLedgerCreateDto();
    this.model.ParcelNumbers = [];

    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
      this.currentUser = currentUser;

      forkJoin(
        this.waterTypeService.getWaterTypes(),
        this.parcelService.getParcelAllocationAndUsagesByYear(new Date().getFullYear())
      ).subscribe(([waterTypes, parcelAllocationAndUsagesByYear]) => {
        this.waterTypes = waterTypes.filter(x => x.IsUserDefined);
        this.parcelAllocationAndUsagesByYear = parcelAllocationAndUsagesByYear;

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

  public onGridReady(params) {
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
  }

  private initializeParcelSelectGrid() {
    let _decimalPipe = this.decimalPipe;
    this.columnDefs = [
      { filter: false, sortable: false, checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true },
      { 
        headerName: 'APN', 
        valueGetter: function (params: any) { return { LinkValue: params.data.ParcelID, LinkDisplay: params.data.ParcelNumber }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/parcels/" },
        filterValueGetter: function (params) { return params.data.ParcelNumber; },  
      },
      {
        headerName: 'Area (acres)', cellStyle: { textAlign: 'right'}, filterParams: { defaultOption: 'equals' },
        valueGetter: function(params: any) { return parseFloat(_decimalPipe.transform(params.data.ParcelAreaInAcres, '1.1-1')); }
      },
      { 
        headerName: 'Account', 
        valueGetter: function (params: any) { return { LinkValue: params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner.AccountDisplayName }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: function (params) { return params.data.LandOwner.AccountDisplayName; }, 
      },
      { 
        headerName: 'Total Allocation', cellStyle: { textAlign: 'right'}, filterParams: { defaultOption: 'equals' },
        valueGetter: function(params: any) { return _decimalPipe.transform(params.data.Allocation, '1.1-1'); }
      }
    ];

    this.defaultColDef = { sortable: true, filter: true }
    this.gridOptions = { rowSelection: 'multiple', onRowSelected: (event: RowSelectedEvent) => this.onParcelSelectionChange(event) }
  }

  private insertWaterTypeColDefs() {
    let _decimalPipe = this.decimalPipe;
    let colDefsWithWaterTypes = this.columnDefs;

    this.waterTypes.forEach(waterType => {
      colDefsWithWaterTypes.push(
        {
          headerName: waterType.WaterTypeName, field: 'Allocations[waterType.WaterTypeID]', cellStyle: { textAlign: 'right'}, filterParams: { defaultOption: 'equals' },
          valueGetter: function (params) { return !params.data.Allocations[waterType.WaterTypeID] ? 0.0 : 
            _decimalPipe.transform(params.data.Allocations[waterType.WaterTypeID], "1.1-1");
          },
        }
      );
      this.gridApi.setColumnDefs(colDefsWithWaterTypes);
    });
    this.parcelSelectGrid.api.setRowData(this.parcelAllocationAndUsagesByYear);
    this.columnApi.autoSizeAllColumns();
  }

  private clearErrorAlerts() {
    if (!this.alertsCountOnLoad) {
      this.alertsCountOnLoad = this.alertService.getAlerts().length;
    }

    this.alertService.removeAlertsSubset(this.alertsCountOnLoad, this.alertService.getAlerts().length - this.alertsCountOnLoad);
  }

  public onParcelSelectionChange(e: RowSelectedEvent) {
    const parcelNumber = e.data.ParcelNumber;
    const index = this.model.ParcelNumbers.indexOf(parcelNumber);
    
    if (index > -1) {
      this.model.ParcelNumbers.splice(index, 1);
    } else {
      this.model.ParcelNumbers.push(parcelNumber);
    }

    this.noParcelsSelected = this.model.ParcelNumbers.length === 0;
  }

  public onSubmit(createTransactionForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.clearErrorAlerts();

    this.model.TransactionTypeID = TransactionTypeEnum.Supply;
    debugger;

    this.parcelLedgerService.newBulkTransaction(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        createTransactionForm.reset();
        
        this.router.navigateByUrl("/parcels/create-water-transactions").then(x => {
            this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
          });
        },
        error => {
          this.isLoadingSubmit = false;
          console.log(error);
          this.cdr.detectChanges();
        }
      );
  }
}
