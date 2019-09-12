import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ColDef } from 'ag-grid-community';
import { UserLinkRendererComponent } from 'src/app/shared/components/ag-grid/user-link-renderer/user-link-renderer.component';
import { LandownerDashboardLinkRendererComponent } from 'src/app/shared/components/ag-grid/landowner-dashboard-link-renderer/landowner-dashboard-link-renderer.component';
import { DecimalPipe } from '@angular/common';

@Component({
    selector: 'rio-user-list',
    templateUrl: './user-list.component.html',
    styleUrls: ['./user-list.component.scss']
})
export class UserListComponent implements OnInit, OnDestroy {
    private watchUserChangeSubscription: any;
    private currentUser: UserDto;

    public rowData = [];
    columnDefs: ColDef[];  

    constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private userService: UserService, private decimalPipe: DecimalPipe) { }

    ngOnInit() {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
            this.currentUser = currentUser;
            this.userService.getUsers().subscribe(users => {
                let _decimalPipe = this.decimalPipe;

                this.columnDefs = [
                    { headerName: '', field: 'UserID', cellRendererFramework: LandownerDashboardLinkRendererComponent, sortable: false, filter: false, width: 40 },
                    {
                        headerName: 'Name', valueGetter: function(params) {
                            return {
                                FirstName: params.data.FirstName,
                                LastName: params.data.LastName,
                                FullName: params.data.FullName,
                                UserID: params.data.UserID
                            }
                        }, cellRendererFramework: UserLinkRendererComponent,
                        cellRendererParams: { inRouterLink: "/users/" },
                        filterValueGetter: function (params: any) {
                          return params.data.FullName;
                        },
                        comparator: function (id1: any, id2: any) {
                          let user1 = id1 ? id1.LastName + ", " + id1.FirstName : '';
                          let user2 = id2 ? id2.LastName + ", " + id2.FirstName : '';
                          if (user1 < user2) {
                            return -1;
                          }
                          if (user1 > user2) {
                            return 1;
                          }
                          return 0;
                        },
                        sortable: true, filter: true
                      },
                    { headerName: 'Email', field: 'Email', sortable: true, filter: true },
                    { headerName: 'Role', field: 'RoleDisplayName', sortable: true, filter: true, width: 100 },
                    { headerName: 'Has Active Trades?', valueGetter: function (params) { return params.data.HasActiveTrades ? "Yes" : "No"; }, sortable: true, filter: true, width: 160 },
                    { headerName: 'Water Purchased (ac-ft)', field: 'AcreFeetOfWaterPurchased', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 200 },
                    { headerName: 'Water Sold (ac-ft)', field: 'AcreFeetOfWaterSold', valueFormatter: function (params) { return _decimalPipe.transform(params.value, '1.0'); }, sortable: true, filter: true, width: 160 },
                ];
                  this.rowData = users;
            });
        });
    }

    ngOnDestroy() {
        this.watchUserChangeSubscription.unsubscribe();
        this.authenticationService.dispose();
        this.cdr.detach();
    }
}
