import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { GridOptions } from 'ag-grid-community';
import { AccountReconciliationService } from 'src/app/services/account-reconciliation-service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { MultiLinkRendererComponent } from 'src/app/shared/components/ag-grid/multi-link-renderer/multi-link-renderer.component';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-account-reconciliation',
  templateUrl: './account-reconciliation.component.html',
  styleUrls: ['./account-reconciliation.component.scss']
})
export class AccountReconciliationComponent implements OnInit {

  @ViewChild('accountReconciliationGrid') accountReconciliationGrid: AgGridAngular;

  public richTextTypeID: number = CustomRichTextType.AccountReconciliationReport;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public gridOptions: GridOptions;
  public rowData = [];
  public mapHeight: string = "500px"
  public columnDefs: any;

  public gridApi: any;
  public highlightedParcel: any;
  public selectedParcelIDs: Array<number> = [];

  private _highlightedParcelID: number;
  public loadingParcels: boolean = true;
  public selectedParcelsLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Account Parcels";
  public set highlightedParcelID(value: number) {
    if (value != this._highlightedParcelID) {
      this._highlightedParcelID = value;
      this.highlightedParcel = this.rowData.filter(x => x.ParcelID == value)[0];
      this.selectHighlightedParcelIDRowNode();
    }
  }
  public get highlightedParcelID() {
    return this._highlightedParcelID;
  }

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private accountReconciliationService: AccountReconciliationService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {

      this.columnDefs = [
        {
          headerName: 'APN', valueGetter: function (params: any) {
            return { LinkDisplay: params.data.Parcel.ParcelNumber, LinkValue: params.data.Parcel.ParcelID };
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
          sortable: true, filter: true, width:100
        },
        {
          headerName: 'Previously Associated Account', valueGetter: function (params: any) {
            return { LinkDisplay: params.data.LastKnownOwner?.AccountName, LinkValue: params.data.LastKnownOwner?.AccountID };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/accounts/" },
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
          sortable: true, filter: true, width:300
        },
        {
          headerName: 'Associated Ownership Records',
          valueGetter: function (params) {
            let names = params.data.AccountsClaimingOwnership?.map(x => {
              return { LinkValue: x.AccountID, LinkDisplay: x.AccountName }
            });
            const downloadDisplay = names?.map(x => x.LinkDisplay).join(", ");

            return { links: names, DownloadDisplay: downloadDisplay ?? "" };
          },
          filterValueGetter: function (params: any) {
            let names = params.data.AccountsClaimingOwnership?.map(x => {
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
          , sortable: true, filter: true, width:700, cellRendererParams: { inRouterLink: "/accounts/" }, cellRendererFramework: MultiLinkRendererComponent
        },
      ];

      this.gridOptions = <GridOptions>{};
      this.currentUser = currentUser;
      this.accountReconciliationGrid.api.showLoadingOverlay();

      this.accountReconciliationService.getAccountsToBeReconciled().subscribe((parcels) => {
        this.rowData = parcels;
        this.selectedParcelIDs = this.rowData.map(x => x.Parcel.ParcelID);
        this.accountReconciliationGrid.api.hideOverlay();
        setTimeout(() => {
          this.accountReconciliationGrid.columnApi.autoSizeAllColumns();
        }, 50);
        this.loadingParcels = false;
        this.cdr.detectChanges();
      });

    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.accountReconciliationGrid, 'inactive-parcels.csv', null);
  }

  public onGridReady(params) {
    this.gridApi = params.api;
  }

  public onSelectionChanged(event) {
    let selection = this.gridApi.getSelectedRows()[0];
    if (selection && selection.Parcel.ParcelID) {
      this.highlightedParcelID = selection.Parcel.ParcelID;
    }
  }

  public selectHighlightedParcelIDRowNode() {
    this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
      if (rowNode.data.Parcel.ParcelID == this.highlightedParcelID) {
        rowNode.setSelected(true);
        this.gridApi.ensureIndexVisible(index);
      }
    });
  }

}
