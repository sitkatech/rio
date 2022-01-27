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
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { NgbDateAdapter, NgbDateNativeAdapter } from '@ng-bootstrap/ng-bootstrap';


@Component({
  selector: 'rio-parcel-ledger-create-from-spreadsheet',
  templateUrl: './parcel-ledger-create-from-spreadsheet.component.html',
  styleUrls: ['./parcel-ledger-create-from-spreadsheet.component.scss'],
  providers: [{provide: NgbDateAdapter, useClass: NgbDateNativeAdapter}]
})
export class ParcelLedgerCreateFromSpreadsheetComponent implements OnInit {

  
  private currentUser: UserDto;
  public richTextTypeID = CustomRichTextType.ParcelLedgerCreateFromSpreadsheet;
  public waterTypes: WaterTypeDto[];
  public isLoadingSubmit: boolean = false;
 
  public inputtedFile: any;
  public effectiveDate: Date;
  public waterTypeID: number;

  constructor(
    private waterTypeService: WaterTypeService,
    private authenticationService: AuthenticationService,
    private parcelLedgerService: ParcelLedgerService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService,
    private router: Router,
    private datePipe: DatePipe,
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

  private validateEffectiveDateAndWaterTypeID() {
    var formValid = true;
    if (!this.effectiveDate) {
      this.alertService.pushAlert(new Alert('EffectiveDate: The Effective Date field is required', AlertContext.Danger));
      formValid = false;
    }
    if (!this.waterTypeID) {
      this.alertService.pushAlert(new Alert('SupplyType: The Supply Type field is required.', AlertContext.Danger));
      formValid = false;
    }
    return formValid;
  }

  public onSubmit(createTransactionFromSpreadsheetForm: HTMLFormElement) { 
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();

    if (!this.validateEffectiveDateAndWaterTypeID()) {
      this.isLoadingSubmit = false;
      window.scroll(0,0);
      return;
    }

    const _datePipe = this.datePipe;
    const effectiveDateString = (<HTMLInputElement> document.getElementById("effective-date")).value;

    this.parcelLedgerService.newCSVUploadTransaction(this.inputtedFile, effectiveDateString, this.waterTypeID)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        createTransactionFromSpreadsheetForm.reset();

        this.router.navigateByUrl("/parcels/create-water-transactions").then(x => {
          this.alertService.pushAlert(new Alert(response + " transactions were successfully created.", AlertContext.Success));
        });
      },
      error => {
        this.isLoadingSubmit = false;
        if (error.error.errors && error.error.errors.UploadedFile) {
          this.inputtedFile = null;
          (<HTMLInputElement>this.document.getElementById("CSV-upload")).value = null;
          this.alertService.pushAlert(new Alert(error.error.errors.UploadedFile, AlertContext.Danger));
        }
        if (error.error.UploadedFile) {
          this.inputtedFile = null;
          (<HTMLInputElement>this.document.getElementById("CSV-upload")).value = null;
          this.alertService.pushAlert(new Alert(error.error.UploadedFile, AlertContext.Danger));
        }
        if (error.error.EffectiveDate) {
          this.alertService.pushAlert(new Alert(error.error.EffectiveDate, AlertContext.Danger));
        }
        window.scroll(0,0);
        this.cdr.detectChanges();
      });
    }
}
