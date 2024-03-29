import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { DatePipe, DOCUMENT } from '@angular/common';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelLedgerService } from 'src/app/services/parcel-ledger.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { NgbDateAdapter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateAdapterFromString } from 'src/app/shared/components/ngb-date-adapter-from-string';
import { ApiService } from 'src/app/shared/services';


@Component({
  selector: 'rio-parcel-ledger-csv-upload-supply',
  templateUrl: './parcel-ledger-csv-upload-supply.component.html',
  styleUrls: ['./parcel-ledger-csv-upload-supply.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateAdapterFromString}]
})
export class ParcelLedgerCsvUploadSupplyComponent implements OnInit {
  private currentUser: UserDto;

  public richTextTypeID = CustomRichTextTypeEnum.ParcelLedgerCsvUploadSupply;
  public waterTypes: WaterTypeDto[];
  public isLoadingSubmit: boolean = false;
 
  public inputtedFile: any;
  public effectiveDate = '';
  public waterTypeID = '';

  constructor(
    private waterTypeService: WaterTypeService,
    private authenticationService: AuthenticationService,
    private parcelLedgerService: ParcelLedgerService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService,
    private router: Router,
    private apiService: ApiService,
    @Inject(DOCUMENT) private document: Document
  ) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe((currentUser) => {
      this.currentUser = currentUser;

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
      });
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
  }

  private getSelectedFile(event: any) {
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      return event.target.files.item(0);
    }
    return null;
  }

  public onCSVFileChange(event: any) {
    this.inputtedFile = this.getSelectedFile(event);
  }

  public onClickFileInput() {
    this.document.getElementById("CSV-upload").click();
  }

  public onSubmit(createTransactionFromSpreadsheetForm: HTMLFormElement) { 
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();

    this.parcelLedgerService.newCSVUploadTransaction(this.inputtedFile, this.effectiveDate, this.waterTypeID)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        createTransactionFromSpreadsheetForm.reset();

        this.router.navigateByUrl("/parcels/create-water-transactions").then(x => {
          this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
        });
      },
      error => {
        this.isLoadingSubmit = false;
        this.apiService.sendErrorToHandleError(error);
        window.scroll(0,0);
        this.cdr.detectChanges();
      });
    }
}
