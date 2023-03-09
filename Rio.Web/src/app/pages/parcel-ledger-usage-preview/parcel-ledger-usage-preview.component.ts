import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelUsageService } from 'src/app/services/parcel-usage.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { ParcelUsageStagingSimpleDto } from 'src/app/shared/generated/model/parcel-usage-staging-simple-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
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

  constructor(
    private router: Router,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private parcelUsageService: ParcelUsageService,
    private alertService: AlertService,
    private datePipe: DatePipe,
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
    console.log("deleting staged records")
    this.parcelUsageService.deleteStagedParcelUsage().subscribe(() => {
      console.log("did it")
    });
  }

  private createColumnDefs() {
    var _datePipe = this.datePipe;
    var month = _datePipe.transform(this.stagedParcelUsages[0].ReportedDate, 'MMM');

    this.columnDefs = [
      { headerName: 'APN', field: 'ParcelNumber', width: 140 },
      this.utilityFunctionsService.createDecimalColumnDef(`Current Monthly Usage (${month})`, 'ExistingMonthlyUsageAmount', 210, 3),
      this.utilityFunctionsService.createDecimalColumnDef('Current Annual Usage', 'ExistingAnnualUsageAmount', 170, 3),
      this.utilityFunctionsService.createDecimalColumnDef('Usage Adjustment (ac-ft)', 'ReportedValueInAcreFeet', 200, 3),
      this.utilityFunctionsService.createDecimalColumnDef(`Updated Monthly Usage (${month})`, 'UpdatedMonthlyUsageAmount', 210, 3),
      this.utilityFunctionsService.createDecimalColumnDef('Updated Annual Usage', 'UpdatedAnnualUsageAmount', 170, 3),
    ];

    this.parcelNumbersWithoutStagedUsagesColumnDefs = [
      { headerName: 'APN', field: 'ParcelNumber' }
    ];

    this.defaultColDef = { filter: true, sortable: true, resizable: true };
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

    // this.parcelUsageService.deleteStagedParcelUsage().subscribe(() => {
    // });
  }
}
