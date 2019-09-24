import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DecimalPipe } from '@angular/common';
import { ColDef } from 'ag-grid-community';
import { ParcelNumberLinkRendererComponent } from 'src/app/shared/components/ag-grid/parcel-number-link-renderer/parcel-number-link-renderer.component';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public rowData = [];
  columnDefs: any;

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private parcelService: ParcelService,
    private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.parcelService.getParcelsWithWaterUsage().subscribe(result => {
        this.rowData = result;
        this.cdr.detectChanges();
      });

      let _decimalPipe = this.decimalPipe;
      this.columnDefs = [
        {
          headerName: 'Parcel Details',
          children: [
            {
              headerName: 'APN', valueGetter: function (params: any) {
                return { ParcelNumber: params.data.ParcelNumber, ParcelID: params.data.ParcelID };
              }, cellRendererFramework: ParcelNumberLinkRendererComponent,
              cellRendererParams: { inRouterLink: "/parcels/" },
              filterValueGetter: function (params: any) {
                return params.data.ParcelNumber;
              },
              comparator: function (id1: any, id2: any) {
                if (id1.ParcelNumber < id2.ParcelNumber) {
                  return -1;
                }
                if (id1.ParcelNumber > id2.ParcelNumber) {
                  return 1;
                }
                return 0;
              },
              sortable: true, filter: true, width: 100
            },
            { headerName: 'Area (acres)', field: 'ParcelAreaInAcres', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 120 },
            { headerName: 'In Pilot?', valueGetter: function (params) { return params.data.LandOwner !== null ? "Yes" : "No"; }, sortable: true, filter: true, width: 100 },
            {
              headerName: 'Land Owner', valueGetter: function (params: any) {
                return { LinkValue: params.data.LandOwner.UserID, LinkDisplay: params.data.LandOwner.FullName };
              }, cellRendererFramework: LinkRendererComponent,
              cellRendererParams: { inRouterLink: "/users/" },
              filterValueGetter: function (params: any) {
                return params.data.Buyer.FullName;
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
          ]
        },
        {
          headerName: '2018 Water Data (acre-feet)',
          children: [
            { headerName: 'Allocation', field: 'AllocationFor2018', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
            { headerName: 'Usage', field: 'WaterUsageFor2018', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
          ]
        },
        {
          headerName: '2017 Water Data (acre-feet)',
          children: [
            { headerName: 'Allocation', field: 'AllocationFor2017', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
            { headerName: 'Usage', field: 'WaterUsageFor2017', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
          ]
        },
        {
          headerName: '2016 Water Data (acre-feet)',
          children: [
            { headerName: 'Allocation', field: 'AllocationFor2016', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
            { headerName: 'Usage', field: 'WaterUsageFor2016', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 100 },
          ]
        }
      ];


    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }
}
