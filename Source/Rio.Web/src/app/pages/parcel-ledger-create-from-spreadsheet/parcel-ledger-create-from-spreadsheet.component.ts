import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-parcel-ledger-create-from-spreadsheet',
  templateUrl: './parcel-ledger-create-from-spreadsheet.component.html',
  styleUrls: ['./parcel-ledger-create-from-spreadsheet.component.scss']
})
export class ParcelLedgerCreateFromSpreadsheetComponent implements OnInit {

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public richTextTypeID = CustomRichTextType.ParcelLedgerCreateFromSpreadsheet;
  public waterTypes: WaterTypeDto[];
  public isLoadingSubmit: boolean = false;
  public fileUpload: File;

  constructor(
    private waterTypeService: WaterTypeService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
  ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe((currentUser) => {
      this.currentUser = currentUser;

      this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
        // only grab user defined water types
        this.waterTypes = waterTypes.filter(x => x.IsUserDefined);
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public onSubmit(createTransactionFromSpreadsheetForm: HTMLFormElement) { }
}
