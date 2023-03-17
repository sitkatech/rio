import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { ParcelUsageService } from 'src/app/services/parcel-usage.service';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateAdapterFromString } from 'src/app/shared/components/ngb-date-adapter-from-string';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { ApiService } from 'src/app/shared/services';

@Component({
  selector: 'parcel-ledger-csv-upload-usage',
  templateUrl: './parcel-ledger-csv-upload-usage.component.html',
  styleUrls: ['./parcel-ledger-csv-upload-usage.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateAdapterFromString}]
})
export class ParcelLedgerCsvUploadUsageComponent implements OnInit {
  private currentUser: UserDto;

  public fileUpload: File;
  public fileUploadHeaders: string[];
  public fileUploadElementID = "file-upload";
  public fileUploadElement: HTMLInputElement;

  public apnColumnName: string;
  public quantityColumnName: string;
  public effectiveDate: string;

  public displayFileInputPanel = true;
  public isLoadingSubmit: boolean = false;
  public richTextTypeID = CustomRichTextTypeEnum.ParcelLedgerCsvUploadUsage;

  constructor(
    private authenticationService: AuthenticationService,
    private router: Router,
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private parcelUsageService: ParcelUsageService,
    private alertService: AlertService,
    private apiService: ApiService
  ) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
  }

  public onFileUploadChange(event: any) {
    if (!event.target.files || !event.target.files.length) {
      this.fileUpload = null;
      event.target.value = null;
    }

    const [file] = event.target.files;
    this.fileUpload = event.target.files.item(0);
  }

  public onClickFileUpload() {
    if (!this.fileUploadElement) {
      this.fileUploadElement = <HTMLInputElement>document.getElementById(this.fileUploadElementID);
    }
    this.fileUploadElement.click();
    this.fileUpload = null;
  }

  public getFileUploadHeaders() {
    if (!this.fileUpload) {
      this.alertService.pushAlert(new Alert("The File field is required.", AlertContext.Danger));
      return;
    }

    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();

    this.parcelUsageService.getCsvHeaders(this.fileUpload).subscribe(fileUploadHeaders => {
      this.isLoadingSubmit = false;
      this.fileUploadHeaders = fileUploadHeaders;
      this.displayFileInputPanel = false;

    }, error => {
      this.isLoadingSubmit = false;
      this.apiService.sendErrorToHandleError(error);
    });
  }

  public backToFileInputPanel() {
    this.displayFileInputPanel = true;
  }

  private validateRequiredFields(): boolean {
    var isValid = true;
    
    if (!this.apnColumnName) {
      this.alertService.pushAlert(new Alert("The APN Column field is required.", AlertContext.Danger));
      isValid = false;
    }
    if (!this.quantityColumnName) {
      this.alertService.pushAlert(new Alert("The Quantity Column field is required.", AlertContext.Danger));
      isValid = false;
    }
    if (!this.effectiveDate) {
      this.alertService.pushAlert(new Alert("The Effective Date field is required.", AlertContext.Danger));
      isValid = false;
    }

    return isValid;
  }

  public onSubmit() { 
    this.alertService.clearAlerts();

    if (!this.validateRequiredFields()) return;

    this.isLoadingSubmit = true;

    this.parcelUsageService.uploadParcelUsageCsvToStaging(this.fileUpload, this.effectiveDate, this.apnColumnName, this.quantityColumnName)
      .subscribe(parcelUsageCsvResponseDto => {
        this.isLoadingSubmit = false;

        this.router.navigate(['preview'], {relativeTo: this.route}).then(() => {
          if (parcelUsageCsvResponseDto.UnmatchedParcelNumbers?.length > 0) {
            const unmatchedAPNsMessage = `${parcelUsageCsvResponseDto.UnmatchedParcelNumbers?.length} transactions skipped because the APNs were found in the file and are not in the Water Accounting Database: ${parcelUsageCsvResponseDto.UnmatchedParcelNumbers}`
            this.alertService.pushAlert(new Alert(unmatchedAPNsMessage, AlertContext.Info, false));
          }
          if (parcelUsageCsvResponseDto.NullParcelNumberCount > 0) {
            const nullAPNsMessage = `${parcelUsageCsvResponseDto.NullParcelNumberCount} <X> transactions were skipped because an APN was not provided in the specified column`
            this.alertService.pushAlert(new Alert(nullAPNsMessage, AlertContext.Info, false));
          }
        });
      }, error => {
        this.isLoadingSubmit = false;
        this.apiService.sendErrorToHandleError(error);

        if (error.error?.UploadedFile) {
          this.fileUpload = null;
          this.fileUploadElement.value = null;

          this.displayFileInputPanel = true;
        }
        this.cdr.detectChanges();
      });
    }
}
