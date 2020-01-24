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

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  @ViewChild('parcelsGrid', {static: false}) parcelsGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public waterYears: Array<number> = [];
  public waterYearToDisplay: number;
  public gridOptions: GridOptions;
  public rowData = [];
  columnDefs: any;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private parcelService: ParcelService,
    private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.gridOptions = <GridOptions>{};
      this.currentUser = currentUser;
      this.waterYearToDisplay = (new Date()).getFullYear();
      forkJoin(
        this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay), 
        this.parcelService.getWaterYears()
      ).subscribe(([parcelsWithWaterUsage, waterYears]) => {
        this.rowData = parcelsWithWaterUsage;
        this.waterYears = waterYears;
        this.cdr.detectChanges();
      });

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
          { headerName: 'In Pilot?', valueGetter: function (params) { return params.data.LandOwner !== null ? "Yes" : "No"; }, sortable: true, filter: true, width: 100 },
          {
            headerName: 'Landowner', valueGetter: function (params: any) {
              return { LinkValue: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountDisplayName };
            }, cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/accounts/" },
            filterValueGetter: function (params: any) {
              return (params.data.LandOwner) ? params.data.LandOwner.FullName : null;
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
        { headerName: 'Native Yield', field: 'NativeYield', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
        { headerName: 'Project Water', field: 'ProjectWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
        { headerName: 'Stored Water', field: 'StoredWater', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
        { headerName: 'Reconciliation', field: 'Reconciliation', valueFormatter: function (params) { return _decimalPipe.transform(params.value, "1.1-1"); }, sortable: true, filter: true, width: 130 },
      ];
      
      this.columnDefs.forEach(x => {
        x.resizable = true;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public updateGridData(){
    this.parcelService.getParcelAllocationAndUsagesByYear(this.waterYearToDisplay).subscribe(result => {
        this.parcelsGrid.api.setRowData(result);
    });
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelsGrid, 'parcels.csv', null);
  }
}
