import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DecimalPipe } from '@angular/common';
import { GridOptions } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { forkJoin } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  @ViewChild('parcelsGrid') parcelsGrid: AgGridAngular;

  public richTextTypeID: number = CustomRichTextType.ParcelList;

  
  private currentUser: UserDto;

  public waterYears: Array<WaterYearDto>;
  public waterYearToDisplay: WaterYearDto;
  public gridOptions: GridOptions;
  public rowData = [];
  public mapHeight: string = "500px"
  public columnDefs: Array<ColDef>;
  public waterTypes: WaterTypeDto[];
  private waterTypeColumnDefsInsertIndex = 4;
  private tradeColumnDefsInsertIndex = 5;

  public gridApi: any;
  public highlightedParcel: any;
  public selectedParcelIDs: Array<number> = [];

  private _highlightedParcelID: number;
  public loadingParcels: boolean = true;
  public selectedParcelsLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Account Parcels";
  public set highlightedParcelID(value : number) {
    if (value != this._highlightedParcelID) {
      this._highlightedParcelID = value;
      this.highlightedParcel = this.rowData.filter(x => x.ParcelID == value)[0];
      this.selectHighlightedParcelIDRowNode();
    }
  }
  public get highlightedParcelID() {
    return this._highlightedParcelID;
  }

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private waterTypeService: WaterTypeService,
    private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      let _decimalPipe = this.decimalPipe;
      this.columnDefs = [
        {
          headerName: 'APN', valueGetter: function (params: any) {
            return { LinkDisplay: params.data.ParcelNumber, LinkValue: params.data.ParcelID };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/parcels/" },
          filterValueGetter: function (params: any) {
            return params.data.ParcelNumber;
          },
          comparator: function (id1: any, id2: any) {
            if (id1.LinkDisplay < id2.LinkDisplay) {
              return -1;
            }
            if (id1.LinkDisplay > id2.LinkDisplay) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true, width: 100
        },
        { headerName: 'Area (acres)', field: 'ParcelAreaInAcres', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 120 },
        //{ headerName: 'Parcel Status', field: 'ParcelStatus.ParcelStatusDisplayName', sortable: true, filter: true, width: 120},
        {
          headerName: 'Account', valueGetter: function (params: any) {
            return { LinkValue: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountDisplayName };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/accounts/" },
          filterValueGetter: function (params: any) {
            return (params.data.LandOwner) ? params.data.LandOwner.AccountDisplayName : null;
          },
          comparator: function (id1: any, id2: any) {
            let link1 = id1.LinkDisplay;
            let link2 = id2.LinkDisplay;
            if (link1 < link2) {
              return -1;
            }
            if (link1 > link2) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true, width: 170
        },
        { headerName: 'Total Supply', field: 'TotalSupply', sortable: true, filter: 'agNumberColumnFilter', width: 150,
          valueGetter: params => params.data.TotalSupply ?? 0,
          valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
          filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.TotalSupply, "1.2-2"))
        },
        { headerName: 'Precipitation', field: 'Precipitation', sortable: true, filter: 'agNumberColumnFilter', width: 130,
          valueGetter: params => params.data.Precipitation ?? 0,
          valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
          filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.Precipitation, "1.2-2"))
        },
        { headerName: 'Total Usage', field: 'UsageToDate', sortable: true, filter: 'agNumberColumnFilter', width: 130,
          valueGetter: params => params.data.UsageToDate ?? 0,
          valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
          filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.TotalUsage, "1.2-2"))
        },
      ];

      if (this.allowTrading()) {
        const tradeColDefs: Array<ColDef> = [
          { headerName: 'Purchased', field: 'Purchased', sortable: true, filter: 'agNumberColumnFilter', width: 130, 
          valueGetter: params => params.data.Purchased ?? 0.0,
          valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
          filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.Purchased, "1.2-2"))
        },
        { headerName: 'Sold', field: 'Sold', sortable: true, filter: 'agNumberColumnFilter', width: 130,
          valueGetter: params => params.data.Sold ?? 0.0,
          valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
          filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.Sold, "1.2-2"))
        }
        ];

        this.columnDefs.splice(this.tradeColumnDefsInsertIndex, 0, ...tradeColDefs);
      }

      this.gridOptions = <GridOptions>{};
      this.currentUser = currentUser;
      this.parcelsGrid?.api.showLoadingOverlay();
      forkJoin([this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterTypeService.getWaterTypes()
      ]).subscribe(([defaultYear, waterTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.waterTypes = waterTypes;

        // finish setting up the column defs based on existing waterTypes before loading data.
        var waterTypeColDefs: Array<ColDef> = [];
        this.waterTypes.forEach(waterType => {
          waterTypeColDefs.push({
            headerName: waterType.WaterTypeName, sortable: true, filter: 'agNumberColumnFilter', width: 130,
            valueGetter: params => params.data.WaterSupplyByWaterType ? (params.data.WaterSupplyByWaterType[waterType.WaterTypeID] ?? 0) : 0,
            valueFormatter: params => _decimalPipe.transform(params.value, "1.2-2"),
            filterValueGetter: params => params.data.WaterSupplyByWaterType && params.data.WaterSupplyByWaterType[waterType.WaterTypeID] ?
              parseFloat(_decimalPipe.transform(params.data.WaterSupplyByWaterType[waterType.WaterTypeID], '1.2-2')) : 0
          });
        });

        this.columnDefs.splice(this.waterTypeColumnDefsInsertIndex, 0, ...waterTypeColDefs)

        this.columnDefs.forEach(x => {
          x.resizable = true;
        });

        // this is necessary because by the time we enter this subscribe, ngOnInit has concluded and the ag-grid has read its column defs
        this.parcelsGrid.api.setColumnDefs(this.columnDefs);

        forkJoin([
          this.parcelService.getParcelWaterSupplyAndUsagesByYear(this.waterYearToDisplay.Year),
          this.waterYearService.getWaterYears()
        ]).subscribe(([parcelsWithWaterUsage, waterYears]) => {
          this.rowData = parcelsWithWaterUsage;
          this.selectedParcelIDs = this.rowData.map(x => x.ParcelID);
          this.parcelsGrid.api.hideOverlay();
          this.loadingParcels = false;
          this.waterYears = waterYears;
          this.cdr.detectChanges();
        });

      });
    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  private allowTrading(): boolean {
    return environment.allowTrading;
  }

  public updateGridData() {
    if (!this.waterYearToDisplay) {
      return;
    }
    this.parcelService.getParcelWaterSupplyAndUsagesByYear(this.waterYearToDisplay.Year).subscribe(result => {
      this.rowData = result;
      this.selectedParcelIDs = this.rowData.map(x => x.ParcelID);
      this.parcelsGrid.api.setRowData(this.rowData);
    });
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelsGrid, 'parcels.csv', null);
  }

  public onGridReady(params) {
    this.gridApi = params.api;
  }

  public onSelectionChanged(event) {
    let selection = this.gridApi.getSelectedRows()[0];
    if (selection && selection.ParcelID) {
      this.highlightedParcelID = selection.ParcelID;
    }
  }

  public selectHighlightedParcelIDRowNode() {
    this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
      if (rowNode.data.ParcelID == this.highlightedParcelID) {
        rowNode.setSelected(true);
        this.gridApi.ensureIndexVisible(index);
      }
    });
  }
}
