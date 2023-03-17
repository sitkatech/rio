import { DatePipe, DecimalPipe } from '@angular/common';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelUsageService } from 'src/app/services/parcel-usage.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { ParcelUsageStagingSimpleDto } from 'src/app/shared/generated/model/parcel-usage-staging-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ApiService } from 'src/app/shared/services';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'rio-parcel-ledger-usage-preview',
  templateUrl: './parcel-ledger-usage-preview.component.html',
  styleUrls: ['./parcel-ledger-usage-preview.component.scss']
})
export class ParcelLedgerUsagePreviewComponent implements OnInit, OnDestroy {
  @ViewChild('stagedParcelUsagesGrid') stagedParcelUsagesGrid: AgGridAngular;
  @ViewChild('parcelNumbersWithoutStagedUsagesGrid') parcelNumbersWithoutStagedUsagesGrid: AgGridAngular;

  private currentUser: UserDto;

  public stagedParcelUsages: ParcelUsageStagingSimpleDto[];
  public effectiveDate: string;
  
  public parcelNumbersWithoutStagedUsages: string[];
  public parcelNumbersWithoutStagedUsagesCount: number;

  public columnDefs: ColDef[];
  public parcelNumbersWithoutStagedUsagesColumnDefs: ColDef[];
  public defaultColDef: ColDef;

  public isLoadingSubmit = false;
  public unsavedChanges = true;

  public richTextTypeID = CustomRichTextTypeEnum.ParcelLedgerUsagePreview;

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private parcelUsageService: ParcelUsageService,
    private alertService: AlertService,
    private decimalPipe: DecimalPipe,
    private utilityFunctionsService: UtilityFunctionsService
  ) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      this.parcelUsageService.previewStagedParcelUsage().subscribe(parcelUsageStagingPreviewDto => {
        this.stagedParcelUsages = parcelUsageStagingPreviewDto.StagedParcelUsages;
        this.effectiveDate = this.stagedParcelUsages.length > 0 ? this.stagedParcelUsages[0].ReportedDate : null;

        this.parcelNumbersWithoutStagedUsages = parcelUsageStagingPreviewDto.ParcelNumbersWithoutStagedUsages;
        this.parcelNumbersWithoutStagedUsagesCount = parcelUsageStagingPreviewDto.ParcelNumbersWithoutStagedUsages.length;

        this.createColumnDefs();
        this.stagedParcelUsagesGrid.api.sizeColumnsToFit();

        this.cdr.detectChanges();
      });

    });
  }

  ngOnDestroy(): void {
    this.cdr.detach;
  }

  public canExit(): boolean {
    return !this.unsavedChanges;
  }

  public onExit(): void {
    this.parcelUsageService.deleteStagedParcelUsage().subscribe();
  }

  private createColumnDefs() {
    var _decimalPipe = this.decimalPipe;

    this.columnDefs = [
      { headerName: 'APN', field: 'ParcelNumber' },
      {
        headerName: 'Transaction Quantity (ac-ft)', 
        filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right' },
        valueGetter: params => params.data.ReportedValueInAcreFeet,
        valueFormatter: params => _decimalPipe.transform(params.value, '1.3-3'),
        filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.ReportedValueInAcreFeet, '1.3-3'))
      },
      {
        headerName: 'Previous Monthly Usage (ac-ft)', 
        filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right' },
        valueGetter: params => params.data.ExistingMonthlyUsageAmount * -1,
        valueFormatter: params => _decimalPipe.transform(params.value, '1.2-2'),
        filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.ExistingMonthlyUsageAmount * -1, '1.3-3'))
      },
      {
        headerName: 'Updated Monthly Usage (ac-ft)', 
        filter: 'agNumberColumnFilter', cellStyle: { textAlign: 'right' },
        valueGetter: params => params.data.UpdatedMonthlyUsageAmount,
        valueFormatter: params => _decimalPipe.transform(params.value, '1.2-2'),
        filterValueGetter: params => parseFloat(_decimalPipe.transform(params.data.UpdatedMonthlyUsageAmount, '1.3-3'))
      }
    ];

    this.defaultColDef = { filter: true, sortable: true, resizable: true, width: 270 };
  }

  public publishStagedParcelUsages() {
    this.isLoadingSubmit = true;

    this.parcelUsageService.publishStagedParcelUsage().subscribe(transactionCount => {
      this.isLoadingSubmit = false;
      this.unsavedChanges = false;

      this.router.navigate(['/parcels/create-water-transactions']).then(() => {
        this.alertService.pushAlert(new Alert(`${transactionCount} transactions successfully created.`, AlertContext.Success));
      });
    }, error => {
      this.isLoadingSubmit = false;

      this.cdr.detectChanges();
    });
  }

  public cancel() {
    this.router.navigate(['/parcels/create-water-transactions']);
  }
}
