import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { UserService } from 'src/app/services/user/user.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { AccountService } from 'src/app/services/account/account.service';
import { ColDef } from 'ag-grid-community';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { DecimalPipe } from '@angular/common';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';

@Component({
  selector: 'rio-account-list',
  templateUrl: './account-list.component.html',
  styleUrls: ['./account-list.component.scss']
})
export class AccountListComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public accounts: Array<AccountDto>

  public rowData = [];
  columnDefs: ColDef[];

  constructor(
    private accountService: AccountService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef, ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.accountService.listAllAccounts().subscribe(accounts => {
        this.accounts = accounts;

        this.columnDefs = [
          {
            headerName: '', field: 'AccountID', cellRendererFramework: FontAwesomeIconLinkRendererComponent,
            cellRendererParams: { inRouterLink: "/accounts/", fontawesomeIconName: 'tasks' },
            sortable: false, filter: false, width: 40
          },
          {
            headerName: 'Account Name', valueGetter: function (params: any) {
              return { LinkValue: params.data.AccountID, LinkDisplay: params.data.AccountName };
            }, cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/accounts/" },
            filterValueGetter: function (params: any) {
              return params.data.AccountName;
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
          { headerName: 'Account Number', field: 'AccountNumber', sortable: true, filter: true, width: 175 },
          {
            headerName: 'Users',
            valueGetter: function (params) {
              let names = params.data.Users.map(x => `${x.FirstName} ${x.LastName}`);
              return names.join(", ");
            }, sortable: true, filter: true, width: 350
          },
          { headerName: 'Notes', field: 'Notes', sortable: true, filter: true, width: 350 }
        ];
        this.rowData = accounts;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }
}
