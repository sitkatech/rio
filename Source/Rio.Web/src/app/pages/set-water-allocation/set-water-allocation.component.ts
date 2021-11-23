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
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeApplicationTypeEnum, WaterTypeDto } from 'src/app/shared/models/water-type-application-type-enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";

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

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: WaterYearDto;
  public waterYears: Array<WaterYearDto>;
  public allocationHistoryEntries: Array<ParcelAllocationHistoryDto>;

  public displayErrors: any = {};
  public displayFileErrors: any = {};

  public fileName: string;
  public waterTypes: WaterTypeDto[];
  public applicationTypeToUpdate: WaterTypeDto;
  public acreageBasedWaterTypes: WaterTypeDto[];
  public spreadsheetDrivenWaterTypes: WaterTypeDto[];
  public waterTypeToUpdate: WaterTypeDto;

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private waterTypeService: WaterTypeService,
    private datePipe: DatePipe,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();

    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeParcelAllocationHistoryGrid();

      forkJoin(this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterYearService.getWaterYears(),
        this.waterTypeService.getWaterTypes()
      ).subscribe(([defaultYear, waterYears, waterTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.waterYears = waterYears;
        this.waterTypes = waterTypes;
        this.acreageBasedWaterTypes = this.waterTypes.filter(x => x.IsAppliedProportionally === WaterTypeApplicationTypeEnum.Proportional);
        this.spreadsheetDrivenWaterTypes = this.waterTypes.filter(x => x.IsAppliedProportionally === WaterTypeApplicationTypeEnum.Spreadsheet);
        this.waterTypes.forEach(x => {
          this.displayErrors[x.WaterTypeName] = false;
          this.displayFileErrors[x.WaterTypeName] = false;
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

  public submitBulkAllocation(waterType: WaterTypeDto): void {
    if (this.doesWaterTypeHaveErrorsAndDisplayErrorsIfNecessary(waterType)) {
      this.waterTypeToUpdate = null;
      return;
    }

    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    
    this.isLoadingSubmit = true;
    const allocationValue = this.waterAllocations.find(x => x.nativeElement.id == this.getWaterTypeLabel(waterType)).nativeElement.value;
    this.model.AcreFeetAllocated = +allocationValue;
    this.model.WaterTypeID = waterType.WaterTypeID;
    this.model.WaterYear = this.waterYearToDisplay.Year;

    this.parcelService.bulkSetAnnualAllocations(this.model, this.currentUser.UserID)
      .subscribe(response => {
        this.waterTypeToUpdate = null;
        this.clearInputs()
        this.isLoadingSubmit = false;
        this.updateParcelAllocationHistoryGrid();
        this.alertService.pushAlert(new Alert("The " + waterType.WaterTypeName + " for Water Year " + this.waterYearToDisplay?.Year + " was successfully allocated for all parcels.", AlertContext.Success));
      }
        ,
        error => {
          this.waterTypeToUpdate = null;
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }

  public uploadAllocationFile(waterType: WaterTypeDto): void {
    if (this.doesWaterTypeHaveErrorsAndDisplayErrorsIfNecessary(waterType)) {
      this.waterTypeToUpdate = null;
      return;
    }

    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    var file = this.getFile(waterType);

    this.isLoadingSubmit = true;
    this.parcelService.bulkSetAnnualAllocationsFileUpload(file, this.waterYearToDisplay.Year, waterType.WaterTypeID).subscribe(x => {
      this.alertService.pushAlert(new Alert(`Successfully set ${waterType.WaterTypeName} allocation for ${this.waterYearToDisplay?.Year}`, AlertContext.Success, true));
      this.updateParcelAllocationHistoryGrid();
      this.isLoadingSubmit = false;
      this.waterTypeToUpdate = null;
      this.clearInputs();
      this.cdr.detectChanges();
    }, error => {
      if (error.error && error.error.validationMessage) {
        this.alertService.pushAlert(new Alert(error.error.validationMessage, AlertContext.Danger, true));
      }
      else {
        this.alertService.pushAlert(new Alert("There was an error uploading the file.", AlertContext.Danger, true));
      }
      this.waterTypeToUpdate = null;
      this.isLoadingSubmit = false;
      this.clearInputs();
      this.cdr.detectChanges();
    });
  }

  public doesWaterTypeHaveErrorsAndDisplayErrorsIfNecessary(waterType: WaterTypeDto): boolean {
    this.turnOffErrors();
    if (this.isAcreageBasedWaterType(waterType)) {
      const allocationValue = this.waterAllocations.find(x => x.nativeElement.id == this.getWaterTypeLabel(waterType)).nativeElement.value;
      if (allocationValue === undefined || allocationValue === null || allocationValue === "") {
        this.chooseErrorToDisplay(waterType);
        return true;
      }
    }

    if (this.isSpreadsheetDrivenWaterType(waterType)) {
      var file = this.getFile(waterType);
      if (!file) {
        this.chooseErrorToDisplay(waterType);
        return true;
      }

      if (file.name.split(".").pop().toUpperCase() != "CSV") {
        this.displayFileErrors[waterType.WaterTypeName] = true;
        return true;
      }
    }
    return false;
  }

  fileEvent(waterType: WaterTypeDto) {
    let file = this.getFile(waterType);
    this.displayErrors[waterType.toString()] = false;

    if (file && file.name.split(".").pop().toUpperCase() != "CSV") {
      this.displayFileErrors[waterType.WaterTypeName] = true;
    } else {
      this.displayFileErrors[waterType.WaterTypeName] = false;
    }

    this.cdr.detectChanges();
  }

  public getFile(fileUploadType: WaterTypeDto): File {
    if (!this.fileUploads) {
      return null;
    }
    var element = this.fileUploads.filter(x => x.nativeElement.id == this.getWaterTypeLabel(fileUploadType))[0];
    if (!element) {
      return null;
    }
    return element.nativeElement.files[0];
  }

  public getFileName(fileUploadType: WaterTypeDto): string {
    let file = this.getFile(fileUploadType);
    if (!file) {
      return ""
    }

    return file.name;
  }

  public openFileUpload(fileUploadType: WaterTypeDto) {
    this.fileUploads.filter(x => x.nativeElement.id == this.getWaterTypeLabel(fileUploadType))[0].nativeElement.click();
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

  public chooseErrorToDisplay(waterType: WaterTypeDto) {
    this.displayErrors[waterType.WaterTypeName] = true;
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

  public isAcreageBasedWaterType(waterType: WaterTypeDto): boolean {
    return this.acreageBasedWaterTypes.some(x => x.WaterTypeID == waterType.WaterTypeID);
  }

  public isSpreadsheetDrivenWaterType(waterType: WaterTypeDto) {
    return this.spreadsheetDrivenWaterTypes.some(x => x.WaterTypeID == waterType.WaterTypeID);
  }

  public getWaterTypeLabel(waterType: WaterTypeDto): string {
    if (!waterType) {
      debugger;
    }
    return waterType.WaterTypeName.replace(" ", "");
  }

  public getLastSetDateForWaterType(waterType: WaterTypeDto): string {
    return this.datePipe.transform(this.allocationHistoryEntries.filter(x => x.Allocation == waterType.WaterTypeName && x.WaterYear == this.waterYearToDisplay.Year).reduce((x, y) => x.Date > y.Date ? x : y, { Date: null }).Date, "M/d/yyyy");
  }

  public setWaterTypeToUpdateAndLaunchModal(modalContent: any, waterType: WaterTypeDto) {
    if (this.doesWaterTypeHaveErrorsAndDisplayErrorsIfNecessary(waterType)) {
      return;
    }
    this.waterTypeToUpdate = waterType;
    this.launchModal(modalContent);
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', ariaLabelledBy: 'updateWaterAllocationModalTitle', backdrop: 'static', keyboard: false });
  }

  public resetWaterTypeToUpdateAndCloseModal(modal: any) {
    this.waterTypeToUpdate = null;
    this.closeModal(modal, "Cancel click")
  }

  public closeModal(modal: any, reason: string) {
    modal.close(reason);
  }
}
