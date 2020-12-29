import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DecimalPipe } from '@angular/common';
import { ColDef, GridOptions } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { forkJoin } from 'rxjs';
import { AgGridAngular } from 'ag-grid-angular';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";
import { WaterYearService } from 'src/app/services/water-year.service';

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  @ViewChild('parcelsGrid') parcelsGrid: AgGridAngular;

  public richTextTypeID: number = CustomRichTextType.ParcelList;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public waterYears: Array<WaterYearDto>;
  public waterYearToDisplay: WaterYearDto;
  public gridOptions: GridOptions;
  public rowData = [];
  public mapHeight: string = "500px"
  public columnDefs: any;
  public parcelAllocationTypes: ParcelAllocationTypeDto[];

  public gridApi: any;
  public highlightedParcel: any;


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
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {


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
        { headerName: 'Usage', field: 'UsageToDate', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
        { headerName: 'Total Allocation', field: 'Allocation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 150 },
      ];

      this.gridOptions = <GridOptions>{};
      this.currentUser = currentUser;
      this.parcelsGrid.api.showLoadingOverlay();
      forkJoin(this.waterYearService.getDefaultWaterYearToDisplay(),
        this.parcelAllocationTypeService.getParcelAllocationTypes()
      ).subscribe(([defaultYear, parcelAllocationTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.parcelAllocationTypes = parcelAllocationTypes;

        // finish setting up the column defs based on existing parcelAllocationTypes before loading data.
        this.parcelAllocationTypes.forEach(parcelAllocationType => {
          this.columnDefs.push({
            headerName: parcelAllocationType.ParcelAllocationTypeName,
            valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); },
            sortable: true,
            filter: true,
            width: 130,
            valueGetter: function (params) {
              return params.data.Allocations[parcelAllocationType.ParcelAllocationTypeID] ?? 0.0;
            }
          })
        });

        this.columnDefs.forEach(x => {
          x.resizable = true;
        });

        // this is necessary because by the time we enter this subscribe, ngOnInit has concluded and the ag-grid has read its column defs
        this.parcelsGrid.api.setColumnDefs(this.columnDefs);

        forkJoin(
          this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay.Year),
          this.waterYearService.getWaterYears(),
          this.parcelAllocationTypeService.getParcelAllocationTypes()
        ).subscribe(([parcelsWithWaterUsage, waterYears, parcelAllocationTypes]) => {
          this.rowData = parcelsWithWaterUsage;
          this.parcelsGrid.api.hideOverlay();
          this.loadingParcels = false;
          this.waterYears = waterYears;
          console.log(parcelsWithWaterUsage);
          this.cdr.detectChanges();
        });

      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public updateGridData() {
    if (!this.waterYearToDisplay) {
      return;
    }
    this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay.Year).subscribe(result => {
      this.rowData = result;
      this.parcelsGrid.api.setRowData(this.rowData);
    });
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelsGrid, 'parcels.csv', null);
  }

  public getSelectedParcelIDs(): number[] {
    return this.rowData.map(x => x.ParcelID);
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
