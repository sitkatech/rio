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
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { DecimalPipe } from '@angular/common';

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
    private parcelLedgerService: ParcelLedgerService,
    private decimalPipe: DecimalPipe
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      
      this.createTransactionHistoryGridColumnDefs();

      this.parcelLedgerService.getAllTransactionHistory().subscribe(transactionHistory => {
        this.transactionHistoryGrid.api.setRowData(transactionHistory);
      });
      
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  public createTransactionHistoryGridColumnDefs() {
    const _decimalPipe = this.decimalPipe;

    this.columnDefs = [
      this.utilityFunctionsService.createDateColumnDef('Transaction Entry Date', 'TransactionDate', 'short', 180),
      this.utilityFunctionsService.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy', 180),
      { headerName: 'Created By', field: 'CreateUserFullName', width: 180 },
      { headerName: 'Supply Type', field: 'WaterTypeName', width: 180 },
      this.utilityFunctionsService.createDecimalColumnDef('Number of Parcels Affected', 'AffectedParcelsCount', 200, 0),
      {
        headerName: 'Quantity (ac-ft/ac)', filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right' }, sortable: true, resizable: true,
        valueGetter: params => params.data.TransactionAmount,
        valueFormatter: params => params.data.TransactionAmount ? _decimalPipe.transform(params.value, '1.2-2') : '-',
        filterValueGetter: params => params.data.TransactionAmount ? parseFloat(_decimalPipe.transform(params.data.TransactionAmount, '1.2-2')) : '-'
      },
      { headerName: "Spreadsheet Data Source", field: 'UploadedFileName'}
    ];

    this.defaultColDef = { sortable: true, filter: true, resizable: true };
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.transactionHistoryGrid, 'transaction-history.csv', null);
  }
}