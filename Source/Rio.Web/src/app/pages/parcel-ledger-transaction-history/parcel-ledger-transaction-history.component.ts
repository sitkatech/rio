import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { TagService } from 'src/app/services/tag/tag.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';

@Component({
  selector: 'rio-parcel-ledger-transaction-history',
  templateUrl: './parcel-ledger-transaction-history.component.html',
  styleUrls: ['./parcel-ledger-transaction-history.component.scss']
})
export class ParcelLedgerTransactionHistoryComponent implements OnInit {
  @ViewChild('transactionHistoryGrid') transactionHistoryGrid: AgGridAngular;

  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public columnDefs: Array<ColDef>;
  public defaultColDef: ColDef;
  public richTextTypeID = CustomRichTextType.TransactionHistory;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      
      this.createTransactionHistoryGridColumnDefs();
      
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  public createTransactionHistoryGridColumnDefs() {
    this.columnDefs = [
      this.utilityFunctionsService.createDateColumnDef('Transaction Entry Date', 'TransactionDate', 'short'),
      this.utilityFunctionsService.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy'),
      { headerName: 'Created By', field: 'CreateUserFullName' },
      { headerName: 'Supply Type', field: 'WaterTypeName' },
      this.utilityFunctionsService.createDecimalColumnDef('Quantity (ac-ft/ac)', 'Quantity'),
      { headerName: "Spreadsheet Data Source", field: 'UploadedFileName'},
      this.utilityFunctionsService.createDecimalColumnDef('Number of Parcels Affected', 'ParcelsAffectedCount', 120, 0)
    ];

    this.defaultColDef = { sortable: true, filter: true, resizable: true };
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.transactionHistoryGrid, 'transaction-history.csv', null);
  }

}
