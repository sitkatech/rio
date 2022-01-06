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
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeUTCAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ParcelAllocationAndUsageDto } from 'src/app/shared/generated/model/parcel-allocation-and-usage-dto';

@Component({
  selector: 'rio-parcel-ledger-bulk-create',
  templateUrl: './parcel-ledger-bulk-create.component.html',
  styleUrls: ['./parcel-ledger-bulk-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeUTCAdapter}]

})
export class ParcelLedgerBulkCreateComponent implements OnInit {
  @ViewChild('parcelSelectGrid') parcelSelectGrid: AgGridAngular;

  
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
  ) { }

  ngOnInit(): void {
    this.model = new ParcelLedgerCreateDto();
    this.model.ParcelNumbers = [];

    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      forkJoin(
        this.waterTypeService.getWaterTypes(),
        this.parcelService.getParcelAllocationAndUsagesByYear(new Date().getFullYear())
      ).subscribe(([waterTypes, parcelAllocationAndUsagesByYear]) => {
        this.waterTypes = waterTypes;
        this.parcelAllocationAndUsagesByYear = parcelAllocationAndUsagesByYear;

        this.insertWaterTypeColDefs();
      });
    });

    this.initializeParcelSelectGrid();
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  public onGridReady(params) {
    this.gridApi = params.api;
    this.columnApi = params.columnApi;
  }

  private initializeParcelSelectGrid() {
    this.columnDefs = [
      { filter: false, sortable: false, checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true },
      { 
        headerName: 'APN', 
        valueGetter: function (params: any) { return { LinkValue: params.data.ParcelID, LinkDisplay: params.data.ParcelNumber }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/parcels/" },
        filterValueGetter: function (params) { return params.data.ParcelNumber; },  
      },
      {
        headerName: 'Area (acres)', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
        valueGetter: params => this.numberColumnValueGetter(params.data.ParcelAreaInAcres)
      },
      { 
        headerName: 'Account', 
        valueGetter: function (params: any) { return { LinkValue: params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner.AccountDisplayName }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: function (params) { return params.data.LandOwner.AccountDisplayName; }, 
      },
      { 
        headerName: 'Total Allocation', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
        valueGetter: params => this.numberColumnValueGetter(params.data.Allocation)
      }
    ];

    this.defaultColDef = { sortable: true, filter: true }
    this.gridOptions = { rowSelection: 'multiple', onRowSelected: (event: RowSelectedEvent) => this.onParcelSelectionChange(event) }
  }

  private insertWaterTypeColDefs() {
    let colDefsWithWaterTypes = this.columnDefs;
    
    this.waterTypes.forEach(waterType => {
      colDefsWithWaterTypes.push(
        {
          headerName: waterType.WaterTypeName, filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right'},
          valueGetter: params => this.numberColumnValueGetter(params.data.Allocations[waterType.WaterTypeID])
        }
      );
      this.gridApi.setColumnDefs(colDefsWithWaterTypes);
    });

    this.parcelSelectGrid.api.setRowData(this.parcelAllocationAndUsagesByYear);
    this.columnApi.autoSizeAllColumns();
  }

  private numberColumnValueGetter(value: number): number {
    return (value === null || value === undefined) ? 0 : parseFloat(value.toFixed(1));
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
          window.scroll(0,0);
          this.cdr.detectChanges();
        }
      );
  }
}
