import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DecimalPipe } from '@angular/common';
import { ColDef } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { LandOwnerLinkRendererComponent } from 'src/app/shared/components/ag-grid/land-owner-link-renderer/land-owner-link-renderer.component';

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
    private decimalPipe: DecimalPipe) {}

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.parcelService.getParcelsWithWaterUsage().subscribe(result => {
        console.log(result);
        this.rowData = result;
        this.cdr.detectChanges();
      });

      let _decimalPipe = this.decimalPipe;
      this.columnDefs = [
        { headerName: 'APN', field: 'ParcelNumber', cellRendererFramework: LinkRendererComponent, cellRendererParams: {inRouterLink: "/parcels/" }, sortable: true, filter: true },
        { headerName: 'Area in Acres', field: 'ParcelAreaInAcres', valueFormatter: function(params) { return _decimalPipe.transform(params.value, '1.1-1');}, sortable: true, filter: true },
        { headerName: 'Included in Pilot?', valueGetter: function(params) { return params.data.LandOwner !== null ? "Yes" : "No"; }, sortable: true, filter: true },
        { headerName: 'Land Owner', field: 'LandOwner', cellRendererFramework: LandOwnerLinkRendererComponent, 
        cellRendererParams: {inRouterLink: "/users/" }, 
        filterValueGetter: function (params: any) { 
          if(params.data.LandOwner)
          {
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
        sortable: true, filter: true },
        { headerName: '2018', field: 'WaterUsageFor2018', valueFormatter: function(params) { return _decimalPipe.transform(params.value, '1.1-1');}, sortable: true, filter: true },
        { headerName: '2017', field: 'WaterUsageFor2017', valueFormatter: function(params) { return _decimalPipe.transform(params.value, '1.1-1');}, sortable: true, filter: true },
        { headerName: '2016', field: 'WaterUsageFor2016', valueFormatter: function(params) { return _decimalPipe.transform(params.value, '1.1-1');}, sortable: true, filter: true },
      ];
    
    
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }
}
