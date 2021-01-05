import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { AccountDto } from 'src/app/shared/models/account/account-dto';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { AccountService } from 'src/app/services/account/account.service';
import { ColDef } from 'ag-grid-community';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { MultiLinkRendererComponent } from 'src/app/shared/components/ag-grid/multi-link-renderer/multi-link-renderer.component';
import { AgGridAngular } from 'ag-grid-angular';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { AccountStatusEnum } from 'src/app/shared/models/enums/account-status-enum';
@Component({
  selector: 'rio-account-list',
  templateUrl: './account-list.component.html',
  styleUrls: ['./account-list.component.scss']
})
export class AccountListComponent implements OnInit, OnDestroy {
  @ViewChild("accountsGrid") accountsGrid: AgGridAngular;
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public accounts: Array<AccountDto>

  public rowData = [];
  public columnDefs: ColDef[];
  public showOnlyActiveAccounts: boolean = true;

  constructor(
    private accountService: AccountService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef, private utilityFunctionsService: UtilityFunctionsService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.accountsGrid.api.showLoadingOverlay();
      this.accountService.listAllAccounts().subscribe(accounts => {
        this.accounts = accounts;
        this.rowData = accounts.filter(x => x.AccountStatus.AccountStatusID === AccountStatusEnum.Active);
        this.accountsGrid.api.hideOverlay();
        this.cdr.detectChanges();
      });

      this.columnDefs = [
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
        { headerName: 'Account Number', field: 'AccountNumber', sortable: true, filter: true, width: 145 },
        { headerName: 'Status', field: 'AccountStatus.AccountStatusDisplayName', sortable: true, filter: true, width: 100 },
        { headerName: 'Verification Key', field: 'AccountVerificationKey', sortable: true, filter: true, width: 145},
        { headerName: 'Verification Key Last Used', field: 'AccountVerificationKeyLastUseDate', sortable: true, filter: true},
        { headerName: '# of Users', field: 'NumberOfUsers', sortable: true, filter: true, width: 100 },
        { headerName: '# of Parcels', field: 'NumberOfParcels', sortable: true, filter: true, width: 100 },
        {
          headerName: 'Users',
          valueGetter: function (params) {
            let names = params.data.Users.map(x => {
              return { LinkValue: x.UserID, LinkDisplay: `${x.FirstName} ${x.LastName}` }
            });
            const downloadDisplay = names.map(x => x.LinkDisplay).join(", ");

            return { links: names, DownloadDisplay: downloadDisplay };
          },
          filterValueGetter: function (params) {
            let names = params.data.Users.map(x => {
              return { LinkValue: x.UserID, LinkDisplay: `${x.FirstName} ${x.LastName}` }
            });
            const downloadDisplay = names.map(x => x.LinkDisplay).join(", ");

            return downloadDisplay;
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
          }, sortable: true, filter: true, width: 315, cellRendererParams: { inRouterLink: "/users/" }, cellRendererFramework: MultiLinkRendererComponent
        },
        { headerName: 'Notes', field: 'Notes', sortable: true, filter: true, width: 315 }
      ];

      this.columnDefs.forEach(x => {
        x.resizable = true;
      });
    });
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public toggleAccountStatusShown(): void {
    this.showOnlyActiveAccounts = !this.showOnlyActiveAccounts;

    if (this.showOnlyActiveAccounts) {
      this.rowData = this.accounts.filter(x => x.AccountStatus.AccountStatusID === AccountStatusEnum.Active);
    } else {
      this.rowData = this.accounts;
    }
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.accountsGrid, 'accounts.csv', null);
  }
  
  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }  
}
