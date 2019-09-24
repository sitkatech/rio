import { Component, OnInit, ViewChildren, QueryList, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { UserService } from 'src/app/services/user/user.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ColDef } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
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
                    { headerName: '', field: 'UserID', cellRendererFramework: FontAwesomeIconLinkRendererComponent, 
                    cellRendererParams: { inRouterLink: "/landowner-dashboard", fontawesomeIconName: 'tasks' },
                    sortable: false, filter: false, width: 40 },
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
