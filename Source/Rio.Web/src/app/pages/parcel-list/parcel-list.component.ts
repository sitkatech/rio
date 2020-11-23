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

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  @ViewChild('parcelsGrid') parcelsGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public waterYears: Array<number> = [];
  public waterYearToDisplay: number;
  public gridOptions: GridOptions;
  public rowData = [];
  public mapHeight: string = "500px"
  public columnDefs: any;
  public parcelAllocationTypes: ParcelAllocationTypeDto[];

  public gridApi: any;
  public highlightedParcel: any;


  private _highlightedParcelID: number;
  public set highlightedParcelID(value : number) {
    if (value != this._highlightedParcelID) {
      this._highlightedParcelID = value;
      this.highlightedParcel = this.rowData.filter(x => x.ParcelID == value)[0];
      this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
        if (rowNode.data.ParcelID == value) {
          rowNode.setSelected(true);
          this.gridApi.ensureIndexVisible(index, 'top');
        }
      })
    }
  }
  public get highlightedParcelID() {
    return this._highlightedParcelID;
  }

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private parcelService: ParcelService,
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
      forkJoin(this.parcelService.getDefaultWaterYearToDisplay(),
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
          this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay),
          this.parcelService.getWaterYears(),
          this.parcelAllocationTypeService.getParcelAllocationTypes()
        ).subscribe(([parcelsWithWaterUsage, waterYears, parcelAllocationTypes]) => {
          this.rowData = parcelsWithWaterUsage;
          this.parcelsGrid.api.hideOverlay();
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
    this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay).subscribe(result => {
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
    console.log(event);
    console.log(this.gridApi.getSelectedRows()[0]);
    let selection = this.gridApi.getSelectedRows()[0];
    if (selection && selection.ParcelID) {
      this.highlightedParcelID = selection.ParcelID;
    }
  }
}
