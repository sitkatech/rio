import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { ParcelAllocationHistoryDto } from 'src/app/shared/models/parcel/parcel-allocation-history-dto';
import { forkJoin } from 'rxjs';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { ParcelAllocationTypeApplicationTypeEnum, ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterYearDto } from 'src/app/shared/models/openet-sync-history-dto';

@Component({
  selector: 'rio-set-water-allocation',
  templateUrl: './set-water-allocation.component.html',
  styleUrls: ['./set-water-allocation.component.scss']
})
export class SetWaterAllocationComponent implements OnInit, OnDestroy {
  @ViewChild('parcelAllocationHistoryGrid') parcelAllocationHistoryGrid: AgGridAngular;
  @ViewChild("updateWaterAllocationModalContent") updateWaterAllocationModalContent

  @ViewChildren('fileUpload') fileUploads: QueryList<any>;
  @ViewChildren('waterAllocation') waterAllocations: QueryList<any>;

  public modalReference: NgbModalRef;

  public parcelAllocationHistoryGridColumnDefs: ColDef[];
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextType.SetWaterAllocation;

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: WaterYearDto;
  public waterYears: Array<WaterYearDto>;
  public allocationHistoryEntries: Array<ParcelAllocationHistoryDto>;

  public displayErrors: any = {};
  public displayFileErrors: any = {};

  public fileName: string;
  public parcelAllocationTypes: ParcelAllocationTypeDto[];
  public applicationTypeToUpdate: ParcelAllocationTypeDto;
  public acreageBasedAllocationTypes: ParcelAllocationTypeDto[];
  public spreadsheetDrivenAllocationTypes: ParcelAllocationTypeDto[];
  public parcelAllocationTypeToUpdate: ParcelAllocationTypeDto;

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private datePipe: DatePipe,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();

    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeParcelAllocationHistoryGrid();

      forkJoin(this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterYearService.getNonFinalizedWaterYears(),
        this.parcelAllocationTypeService.getParcelAllocationTypes()
      ).subscribe(([defaultYear, waterYears, parcelAllocationTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.waterYears = waterYears;
        this.parcelAllocationTypes = parcelAllocationTypes;
        this.acreageBasedAllocationTypes = this.parcelAllocationTypes.filter(x => x.IsAppliedProportionally === ParcelAllocationTypeApplicationTypeEnum.Proportional);
        this.spreadsheetDrivenAllocationTypes = this.parcelAllocationTypes.filter(x => x.IsAppliedProportionally === ParcelAllocationTypeApplicationTypeEnum.Spreadsheet);
        this.parcelAllocationTypes.forEach(x => {
          this.displayErrors[x.ParcelAllocationTypeName] = false;
          this.displayFileErrors[x.ParcelAllocationTypeName] = false;
        })
        this.updateParcelAllocationHistoryGrid();
      });
    });
  }

  private initializeParcelAllocationHistoryGrid(): void {
    let datePipe = this.datePipe;

    this.parcelAllocationHistoryGridColumnDefs = [
      {
        headerName: 'Date', field: 'Date', valueFormatter: function (params: any) {
          return datePipe.transform(params.data.Date, "M/d/yyyy")
        }, sortable: true, filter: true, width: 130
      },
      { headerName: 'Water Year', field: 'WaterYear', sortable: true, filter: true, width: 130 },
      { headerName: 'Allocation', field: 'Allocation', sortable: true, filter: true, width: 150 },
      { headerName: 'Value (ac-ft/ac)', field: 'Value', sortable: true, filter: true, width: 150 },
      { headerName: 'Uploaded Filename', field: 'Filename', sortable: true, filter: true, width: 275 },
      { headerName: 'User', field: 'User', sortable: true, filter: true }
    ];

    this.parcelAllocationHistoryGridColumnDefs.forEach(x => {
      x.resizable = true;
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public submitBulkAllocation(allocationType: ParcelAllocationTypeDto): void {
    if (this.doesParcelAllocationTypeHaveErrorsAndDisplayErrorsIfNecessary(allocationType)) {
      this.parcelAllocationTypeToUpdate = null;
      return;
    }

    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    
    this.isLoadingSubmit = true;
    const allocationValue = this.waterAllocations.find(x => x.nativeElement.id == this.getParcelAllocationTypeLabel(allocationType)).nativeElement.value;
    this.model.AcreFeetAllocated = +allocationValue;
    this.model.ParcelAllocationTypeID = allocationType.ParcelAllocationTypeID;
    this.model.WaterYear = this.waterYearToDisplay.Year;

    this.parcelService.bulkSetAnnualAllocations(this.model, this.currentUser.UserID)
      .subscribe(response => {
        this.parcelAllocationTypeToUpdate = null;
        this.clearInputs()
        this.isLoadingSubmit = false;
        this.updateParcelAllocationHistoryGrid();
        this.alertService.pushAlert(new Alert("The " + allocationType.ParcelAllocationTypeName + " for Water Year " + this.waterYearToDisplay + " was successfully allocated for all parcels.", AlertContext.Success));
      }
        ,
        error => {
          this.parcelAllocationTypeToUpdate = null;
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public uploadAllocationFile(allocationType: ParcelAllocationTypeDto): void {
    if (this.doesParcelAllocationTypeHaveErrorsAndDisplayErrorsIfNecessary(allocationType)) {
      this.parcelAllocationTypeToUpdate = null;
      return;
    }

    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    var file = this.getFile(allocationType);

    this.isLoadingSubmit = true;
    this.parcelService.bulkSetAnnualAllocationsFileUpload(file, this.waterYearToDisplay.Year, allocationType.ParcelAllocationTypeID).subscribe(x => {
      this.alertService.pushAlert(new Alert(`Successfully set ${allocationType.ParcelAllocationTypeName} allocation for ${this.waterYearToDisplay}`, AlertContext.Success, true));
      this.updateParcelAllocationHistoryGrid();
      this.isLoadingSubmit = false;
      this.parcelAllocationTypeToUpdate = null;
      this.clearInputs();
      this.cdr.detectChanges();
    }, error => {
      if (error.error.validationMessage) {
        this.alertService.pushAlert(new Alert(error.error.validationMessage, AlertContext.Danger, true));
      }
      else {
        this.alertService.pushAlert(new Alert("There was an error uploading the file.", AlertContext.Danger, true));
      }
      this.parcelAllocationTypeToUpdate = null;
      this.isLoadingSubmit = false;
      this.clearInputs();
      this.cdr.detectChanges();
    });
  }

  public doesParcelAllocationTypeHaveErrorsAndDisplayErrorsIfNecessary(parcelAllocationType: ParcelAllocationTypeDto): boolean {
    this.turnOffErrors();
    if (this.isAcreageBasedAllocationType(parcelAllocationType)) {
      const allocationValue = this.waterAllocations.find(x => x.nativeElement.id == this.getParcelAllocationTypeLabel(parcelAllocationType)).nativeElement.value;
      if (allocationValue === undefined || allocationValue === null || allocationValue === "") {
        this.chooseErrorToDisplay(parcelAllocationType);
        return true;
      }
    }

    if (this.isSpreadsheetDrivenAllocationType(parcelAllocationType)) {
      var file = this.getFile(parcelAllocationType);
      if (!file) {
        this.chooseErrorToDisplay(parcelAllocationType);
        return true;
      }
    }
    return false;
  }

  fileEvent(allocationType: ParcelAllocationTypeDto) {
    let file = this.getFile(allocationType);
    this.displayErrors[allocationType.toString()] = false;

    if (file && file.name.split(".").pop().toUpperCase() != "CSV") {
      this.displayFileErrors[allocationType.ParcelAllocationTypeName] = true;
    } else {
      this.displayFileErrors[allocationType.ParcelAllocationTypeName] = false;
    }

    this.cdr.detectChanges();
  }

  public getFile(fileUploadType: ParcelAllocationTypeDto): File {
    if (!this.fileUploads) {
      return null;
    }
    var element = this.fileUploads.filter(x => x.nativeElement.id == this.getParcelAllocationTypeLabel(fileUploadType))[0];
    if (!element) {
      return null;
    }
    return element.nativeElement.files[0];
  }

  public getFileName(fileUploadType: ParcelAllocationTypeDto): string {
    let file = this.getFile(fileUploadType);
    if (!file) {
      return ""
    }

    return file.name;
  }

  public openFileUpload(fileUploadType: ParcelAllocationTypeDto) {
    this.fileUploads.filter(x => x.nativeElement.id == this.getParcelAllocationTypeLabel(fileUploadType))[0].nativeElement.click();
  }

  public updateParcelAllocationHistoryGrid(): void {
    this.parcelService.getParcelAllocationHistory().subscribe(parcelAllocationHistory => {
      this.allocationHistoryEntries = parcelAllocationHistory;
      this.parcelAllocationHistoryGrid ? this.parcelAllocationHistoryGrid.api.setRowData(parcelAllocationHistory) : null;
    });
  }

  public clearInputs() {
    this.waterAllocations.forEach(x => {
      x.nativeElement.value = null;
    })
    this.fileUploads.forEach(element => {
      element.nativeElement.value = null;
      element.nativeElement.files = null;
    })
  }

  public chooseErrorToDisplay(allocationType: ParcelAllocationTypeDto) {
    this.displayErrors[allocationType.ParcelAllocationTypeName] = true;
  }

  public turnOffErrors() {
    Object.keys(this.displayErrors).forEach(key => {
      this.displayErrors[key] = false;
    });

    Object.keys(this.displayFileErrors).forEach(key => {
      this.displayFileErrors[key] = false;
    })
  }

  public lowerCaseFirstLetterNoSpaces(lowerFirst: string): string {
    return (lowerFirst.charAt(0).toLowerCase() + lowerFirst.slice(1)).replace(" ", "");
  }

  public isAcreageBasedAllocationType(parcelAllocationType: ParcelAllocationTypeDto): boolean {
    return this.acreageBasedAllocationTypes.some(x => x.ParcelAllocationTypeID == parcelAllocationType.ParcelAllocationTypeID);
  }

  public isSpreadsheetDrivenAllocationType(parcelAllocationType: ParcelAllocationTypeDto) {
    return this.spreadsheetDrivenAllocationTypes.some(x => x.ParcelAllocationTypeID == parcelAllocationType.ParcelAllocationTypeID);
  }

  public getParcelAllocationTypeLabel(parcelAllocationType: ParcelAllocationTypeDto): string {
    if (!parcelAllocationType) {
      debugger;
    }
    return parcelAllocationType.ParcelAllocationTypeName.replace(" ", "");
  }

  public getLastSetDateForParcelAllocationType(parcelAllocationType: ParcelAllocationTypeDto): string {
    return this.datePipe.transform(this.allocationHistoryEntries.filter(x => x.Allocation == parcelAllocationType.ParcelAllocationTypeName && x.WaterYear == this.waterYearToDisplay.Year).reduce((x, y) => x.Date > y.Date ? x : y, { Date: null }).Date, "M/d/yyyy");
  }

  public setAllocationTypeToUpdateAndLaunchModal(modalContent: any, parcelAllocationType: ParcelAllocationTypeDto) {
    if (this.doesParcelAllocationTypeHaveErrorsAndDisplayErrorsIfNecessary(parcelAllocationType)) {
      return;
    }
    this.parcelAllocationTypeToUpdate = parcelAllocationType;
    this.launchModal(modalContent);
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', ariaLabelledBy: 'updateWaterAllocationModalTitle', backdrop: 'static', keyboard: false });
  }

  public resetParcelAllocationTypeToUpdateAndCloseModal(modal: any) {
    this.parcelAllocationTypeToUpdate = null;
    this.closeModal(modal, "Cancel click")
  }

  public closeModal(modal: any, reason: string) {
    modal.close(reason);
  }
}
