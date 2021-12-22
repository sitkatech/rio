import { Component, OnInit, Inject, ChangeDetectorRef } from '@angular/core';
import { DOCUMENT } from '@angular/common';
import { AuthenticationService } from 'src/app/services/authentication.service';
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

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public richTextTypeID = CustomRichTextType.ParcelLedgerCreateFromSpreadsheet;
  public waterTypes: WaterTypeDto[];
  public isLoadingSubmit: boolean = false;
  
  public csvInputFile: File;

  constructor(
    private waterTypeService: WaterTypeService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    @Inject(DOCUMENT) private document: Document
  ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
      this.currentUser = currentUser;

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        this.waterTypes = waterTypes;
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
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
    this.csvInputFile = this.getSelectedFile(event);
  }

  public getCSVInputFile() {
    return this.csvInputFile ? this.csvInputFile.name : "No file chosen...";
  }

  public clickFileInput() {
    this.document.getElementById("CSV-upload").click();
  }

  public onSubmit(createTransactionFromSpreadsheetForm: HTMLFormElement) { }
}
