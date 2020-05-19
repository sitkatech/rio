import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy, ViewChild, OnChanges, SimpleChanges } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ColDef } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { DecimalPipe } from '@angular/common';
import { AgGridAngular } from 'ag-grid-angular';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { UserCreateDto } from 'src/app/shared/models/user/user-create-dto';
import { RoleEnum } from 'src/app/shared/models/enums/role.enum';
import { UserDetailedDto } from 'src/app/shared/models/user/user-detailed-dto';
import { MultiLinkRendererComponent } from 'src/app/shared/components/ag-grid/multi-link-renderer/multi-link-renderer.component';

declare var $:any;

@Component({
  selector: 'rio-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
  @ViewChild('usersGrid') usersGrid: AgGridAngular;
  @ViewChild('unassignedUsersGrid') unassignedUsersGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public rowData = [];
  columnDefs: ColDef[];
  columnDefsUnassigned: ColDef[];
  users: UserDetailedDto[];
  unassignedUsers: UserDetailedDto[];
  public showOnlyActiveUsers: boolean = true;

  constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private utilityFunctionsService: UtilityFunctionsService, private userService: UserService, private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.usersGrid.api.showLoadingOverlay();
      this.userService.getUsers().subscribe(users => {
        this.rowData = users.filter(x => x.RoleID !== RoleEnum.Disabled);
        this.users = users;
        
        this.unassignedUsers = users.filter(u =>{ return u.RoleID === RoleEnum.Unassigned});

        this.usersGrid.api.hideOverlay();
        
        this.cdr.detectChanges();
      });

      let _decimalPipe = this.decimalPipe;

        this.columnDefs = [
          {
            headerName: 'Name', valueGetter: function (params: any) {
              return { LinkValue: params.data.UserID, LinkDisplay: params.data.FullName };
            }, cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/users/" },
            filterValueGetter: function (params: any) {
              return params.data.FullName;
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
          { headerName: 'Email', field: 'Email', sortable: true, filter: true },
          { headerName: 'Role', field: 'RoleDisplayName', sortable: true, filter: true, width: 100 },
          {
            headerName: 'Associated Account(s)',
            valueGetter: function (params) {
              let names = params.data.AssociatedAccounts?.map(x => {
                return { LinkValue: x.AccountID, LinkDisplay: x.AccountName }
              });
              const downloadDisplay = names?.map(x => x.LinkDisplay).join(", ");

              return { links: names, DownloadDisplay: downloadDisplay ?? "" };
            },
            filterValueGetter: function (params: any) {
              let names = params.data.AssociatedAccounts?.map(x => {
                return { LinkValue: x.AccountID, LinkDisplay: x.AccountName }
              });
              const downloadDisplay = names?.map(x => x.LinkDisplay).join(", ");

              return downloadDisplay ?? "";
            },
            comparator: function (id1: any, id2: any) {
              let link1 = id1.DownloadDisplay;
              let link2 = id2.DownloadDisplay;
              if (link1 < link2) {
                return -1;
              }
              if (link1 > link2) {
                return 1;
              }
              return 0;
            }
            , sortable: true, filter: true, width: 315, cellRendererParams: { inRouterLink: "/accounts/" }, cellRendererFramework: MultiLinkRendererComponent
          },
          { headerName: 'Has Active Trades?', valueGetter: function (params) { return params.data.HasActiveTrades ? "Yes" : "No"; }, sortable: true, filter: true, width: 160 },
          { headerName: 'Water Purchased (ac-ft)', field: 'AcreFeetOfWaterPurchased', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 200 },
          { headerName: 'Water Sold (ac-ft)', field: 'AcreFeetOfWaterSold', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 160 },
          { headerName: 'Receives System Communications?', field: 'ReceiveSupportEmails', valueGetter: function (params) { return params.data.ReceiveSupportEmails ? "Yes" : "No";}, sortable: true, filter: true },
        ];
        
        this.columnDefs.forEach(x => {
          x.resizable = true;
        });
    });
  }

  public toggleUserStatusShown(): void {
    this.showOnlyActiveUsers = !this.showOnlyActiveUsers;

    if (this.showOnlyActiveUsers) {
      this.rowData = this.users.filter(x => x.RoleID !== RoleEnum.Disabled);
    } else {
      this.rowData = this.users;
    }
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public exportToCsv() {
    // we need to grab all columns except the first one (trash icon)
    let columnsKeys = this.usersGrid.columnApi.getAllDisplayedColumns(); 
    let columnIds: Array<any> = []; 
    columnsKeys.forEach(keys => 
      { 
        let columnName: string = keys.getColId(); 
        columnIds.push(columnName); 
      });
    columnIds.splice(0, 1);
    this.utilityFunctionsService.exportGridToCsv(this.usersGrid, 'users.csv', columnIds);
  }

  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }  
}
