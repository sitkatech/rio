import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { forkJoin } from 'rxjs';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { ParcelLedgerCreateDto } from 'src/app/shared/generated/model/parcel-ledger-create-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridOptions, RowSelectedEvent } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { TransactionTypeEnum } from 'src/app/shared/generated/enum/transaction-type-enum';
import { NgbDateAdapter, NgbDateNativeUTCAdapter } from '@ng-bootstrap/ng-bootstrap';
import { ParcelWaterSupplyAndUsageDto } from 'src/app/shared/generated/model/parcel-water-supply-and-usage-dto';
import { DecimalPipe } from '@angular/common';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { NgbDateAdapterFromString } from 'src/app/shared/components/ngb-date-adapter-from-string';

@Component({
  selector: 'rio-parcel-ledger-bulk-create',
  templateUrl: './parcel-ledger-bulk-create.component.html',
  styleUrls: ['./parcel-ledger-bulk-create.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateAdapterFromString}]

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

  public richTextTypeID: number = CustomRichTextTypeEnum.ParcelLedgerBulkCreate;
  public searchFailed : boolean = false;
  
  public parcelWaterSupplyAndUsagesByYear: ParcelWaterSupplyAndUsageDto[];
  public columnDefs: ColDef[];
  public defaultColDef: ColDef;
  public gridOptions: GridOptions;
  private tagsColDefInsertIndex = 4;
  
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private parcelService: ParcelService,
    private parcelLedgerService: ParcelLedgerService,
    private waterTypeService: WaterTypeService,
    private utilityFunctionsService: UtilityFunctionsService,
    private decimalPipe: DecimalPipe
  ) { }

  ngOnInit(): void {
    this.model = new ParcelLedgerCreateDto();
    this.model.ParcelNumbers = [];

    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      forkJoin({
        waterTypes: this.waterTypeService.getWaterTypes(),
        parcelWaterSupplyAndUsagesByYear: this.parcelService.getParcelWaterSupplyAndUsagesByYear(new Date().getFullYear())
      }).subscribe(({waterTypes, parcelWaterSupplyAndUsagesByYear}) => {
        this.waterTypes = waterTypes;
        this.parcelWaterSupplyAndUsagesByYear = parcelWaterSupplyAndUsagesByYear;

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
    const _decimalPipe = this.decimalPipe;
    this.columnDefs = [
      { filter: false, sortable: false, checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, width: 40 },
      { 
        headerName: 'APN', width: 140,
        valueGetter: function (params: any) { return { LinkValue: params.data.ParcelID, LinkDisplay: params.data.ParcelNumber }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/parcels/" },
        filterValueGetter: params => params.data.ParcelNumber,  
      },
      this.utilityFunctionsService.createDecimalColumnDef('Area (acres)', 'ParcelAreaInAcres', 120),
      { 
        headerName: 'Account', width: 240,
        valueGetter: function (params: any) { return { LinkValue: params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner.AccountDisplayName }; }, 
        cellRendererFramework: LinkRendererComponent, cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: params => params.data.LandOwner.AccountDisplayName, 
      },
      this.utilityFunctionsService.createDecimalColumnDef('Total Supply', 'TotalSupply', 120)
    ];

    if (this.authenticationService.isCurrentUserAnAdministrator()) {
      const tagsColDef: ColDef =  { headerName: 'Tags', field: 'TagsAsCommaSeparatedString', width: 200 };
      this.columnDefs.splice(this.tagsColDefInsertIndex, 0, tagsColDef);
    }

    this.defaultColDef = { sortable: true, filter: true, resizable: true }
    this.gridOptions = { rowSelection: 'multiple', onRowSelected: (event: RowSelectedEvent) => this.onParcelSelectionChange(event) }
  }

  private insertWaterTypeColDefs() {
    let colDefsWithWaterTypes = this.columnDefs;
    const _decimalPipe = this.decimalPipe;

    this.waterTypes.forEach(waterType => {
      const waterTypeFieldName = 'WaterSupplyByWaterType.' + waterType.WaterTypeID;
      colDefsWithWaterTypes.push(
        this.utilityFunctionsService.createDecimalColumnDef(waterType.WaterTypeName, waterTypeFieldName, 120)
      );
      this.gridApi.setColumnDefs(colDefsWithWaterTypes);
    });

    this.parcelSelectGrid.api.setRowData(this.parcelWaterSupplyAndUsagesByYear);
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
    this.alertService.clearAlerts();
    
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
