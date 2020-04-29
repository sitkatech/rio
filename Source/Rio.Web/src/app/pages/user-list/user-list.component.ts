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

  constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private utilityFunctionsService: UtilityFunctionsService, private userService: UserService, private decimalPipe: DecimalPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.userService.getUsers().subscribe(users => {
        let _decimalPipe = this.decimalPipe;

        this.columnDefs = [
          {
            headerName: '', field: 'UserID', cellRendererFramework: FontAwesomeIconLinkRendererComponent,
            cellRendererParams: { inRouterLink: "/landowner-dashboard", fontawesomeIconName: 'tasks' },
            sortable: false, filter: false, width: 40
          },
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
          { headerName: 'Has Active Trades?', valueGetter: function (params) { return params.data.HasActiveTrades ? "Yes" : "No"; }, sortable: true, filter: true, width: 160 },
          { headerName: 'Water Purchased (ac-ft)', field: 'AcreFeetOfWaterPurchased', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 200 },
          { headerName: 'Water Sold (ac-ft)', field: 'AcreFeetOfWaterSold', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 160 },
          { headerName: 'Receives System Communications?', field: 'ReceiveSupportEmails', valueGetter: function (params) { return params.data.ReceiveSupportEmails ? "Yes" : "No";}, sortable: true, filter: true },
        ];
        
        this.columnDefs.forEach(x => {
          x.resizable = true;
        });

        this.rowData = users;
        this.users = users;
        
        this.unassignedUsers = users.filter(u =>{ return u.RoleID === RoleEnum.Unassigned});

        this.cdr.detectChanges();
      });
    });
  }

  private refreshView(){
    debugger;
    this.unassignedUsersGrid.api.refreshView();
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
}
