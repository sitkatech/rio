import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DecimalPipe } from '@angular/common';
import { ColDef } from 'ag-grid-community';
import { ParcelNumberLinkRendererComponent } from 'src/app/shared/components/ag-grid/parcel-number-link-renderer/parcel-number-link-renderer.component';
import { UserLinkRendererComponent } from 'src/app/shared/components/ag-grid/user-link-renderer/user-link-renderer.component';

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list.component.html',
  styleUrls: ['./parcel-list.component.scss']
})
export class ParcelListComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public rowData = [];
  columnDefs: ColDef[];

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
              sortable: true, filter: true, width: 120
            },
            { headerName: 'Area in Acres', field: 'ParcelAreaInAcres', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 170 },
            { headerName: 'In Pilot?', valueGetter: function (params) { return params.data.LandOwner !== null ? "Yes" : "No"; }, sortable: true, filter: true, width: 150 },
            {
              headerName: 'Land Owner', field: 'LandOwner', cellRendererFramework: UserLinkRendererComponent,
              cellRendererParams: { inRouterLink: "/users/" },
              filterValueGetter: function (params: any) {
                if (params.data.LandOwner) {
                  return params.data.LandOwner.FullName;
                }
                return null;
              },
              comparator: function (id1: any, id2: any) {
                let landOwnerName1 = id1 ? id1.LastName + ", " + id1.FirstName : '';
                let landOwnerName2 = id2 ? id2.LastName + ", " + id2.FirstName : '';
                if (landOwnerName1 < landOwnerName2) {
                  return -1;
                }
                if (landOwnerName1 > landOwnerName2) {
                  return 1;
                }
                return 0;
              },
              sortable: true, filter: true
            }
          ]
        },
        {
          headerName: 'Water Usage',
          children: [
            { headerName: '2018', field: 'WaterUsageFor2018', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 150 },
            { headerName: '2017', field: 'WaterUsageFor2017', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 150 },
            { headerName: '2016', field: 'WaterUsageFor2016', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.1-1'); }, sortable: true, filter: true, width: 150 }
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
