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
import { ParcelAllocationTypeStatic } from 'src/app/shared/models/parcel-allocation-type-static';

@Component({
  selector: 'rio-manage-water-allocation',
  templateUrl: './manage-water-allocation.component.html',
  styleUrls: ['./manage-water-allocation.component.scss']
})
export class ManageWaterAllocationComponent implements OnInit, OnDestroy {
  @ViewChild('parcelAllocationHistoryGrid') parcelAllocationHistoryGrid: AgGridAngular;
  @ViewChild('projectWaterAllocation') projectWaterAllocation: any;
  @ViewChild('nativeYieldAllocation') nativeYieldAllocation: any;
  @ViewChildren('fileUpload') fileUploads:QueryList<any>;
  // @ViewChild('reconciliationWaterFileUpload') reconciliationWaterFileUpload: any;
  // @ViewChild('storedWaterFileUpload') storedWaterFileUpload: any;

  public ParcelAllocationTypeStatic = ParcelAllocationTypeStatic;

  public parcelAllocationHistoryGridColumnDefs: ColDef[];
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: number;
  public waterYears: Array<number>;
  public allocationHistoryEntries: Array<ParcelAllocationHistoryDto>;

  public lastProjectWaterSetDate: string;
  public lastNativeYieldSetDate: string;
  public lastReconciliationFileUploadDate: string;
  public lastStoredWaterFileUploadDate: string;

  public displayErrors:any = {}; 
  public displayFileErrors:any = {};

  public fileName: string;

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();

    Object.keys(ParcelAllocationTypeStatic).forEach(key => {
      this.displayErrors[key] = false;
      this.displayFileErrors[key] = false;
    })

    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeParcelAllocationHistoryGrid();
      this.parcelService.getDefaultWaterYearToDisplay().subscribe(defaultYear => {
        this.waterYearToDisplay = defaultYear;
        this.parcelService.getWaterYearsIncludingCurrentYear().subscribe(waterYears => {
          this.waterYears = waterYears;
          this.updateParcelAllocationHistoryGrid();
        })
      })

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

  public submitBulkAllocation(allocationType: ParcelAllocationTypeStatic, allocationValue: string): void {
    this.isLoadingSubmit = true;

    this.turnOffErrors();

    if (allocationValue !== undefined && allocationValue !== null && allocationValue !== "") {
      this.model.AcreFeetAllocated = +allocationValue;
      this.model.ParcelAllocationTypeID = allocationType.id;
      this.model.WaterYear = this.waterYearToDisplay;

      this.parcelService.bulkSetAnnualAllocations(this.model, this.currentUser.UserID)
        .subscribe(response => {
          this.clearInputs()
          this.isLoadingSubmit = false;
          this.updateParcelAllocationHistoryGrid();
          this.alertService.pushAlert(new Alert("The " + allocationType + " for Water Year " + this.waterYearToDisplay + " was successfully allocated for all parcels.", AlertContext.Success));
        }
          ,
          error => {
            this.isLoadingSubmit = false;
            this.cdr.detectChanges();
          }
        );
    }
    else {
      this.isLoadingSubmit = false;
      this.chooseErrorToDisplay(allocationType);
    }
  }

  public uploadAllocationFile(allocationType: ParcelAllocationTypeStatic): void{
    var file = this.getFile(allocationType);
    if (!file) {
      this.chooseErrorToDisplay(allocationType);
      return null;
    }

    this.isLoadingSubmit = true;
    this.parcelService.bulkSetAnnualAllocationsFileUpload(file, this.waterYearToDisplay, allocationType.id).subscribe(x=>{
      this.alertService.pushAlert(new Alert(`Successfully set ${allocationType} allocation for ${this.waterYearToDisplay}`, AlertContext.Success, true));
      this.updateParcelAllocationHistoryGrid();
      this.isLoadingSubmit = false;
      this.clearInputs();
      this.cdr.detectChanges();
    }, error => {
      if (error.error.validationMessage){
        this.alertService.pushAlert(new Alert(error.error.validationMessage, AlertContext.Danger, true));
      }
      else {
        this.alertService.pushAlert(new Alert("There was an error uploading the file.", AlertContext.Danger, true));
      }
      this.isLoadingSubmit = false;
      this.clearInputs();
      this.cdr.detectChanges();
    });
  }

  fileEvent(allocationType: ParcelAllocationTypeStatic){
    let file = this.getFile(allocationType);
    this.displayErrors[allocationType.toString()] = false;

    if (file && file.name.split(".").pop().toUpperCase() != "CSV"){
      this.displayFileErrors[allocationType.toString()] = true;
    } else {
      this.displayFileErrors[allocationType.toString()] = false;
    }

    this.cdr.detectChanges();
  }

  public getFile(fileUploadType: ParcelAllocationTypeStatic): File{
    if (!this.fileUploads) {
      return null;
    }
    var element = this.fileUploads.filter(x => x.nativeElement.id == this.lowerCaseFirstLetterNoSpaces(fileUploadType.toString()) + "FileUpload")[0];
    if (!element){
      return null;
    }
    return element.nativeElement.files[0];
  }

  public getFileName(fileUploadType: ParcelAllocationTypeStatic):string{
    let file = this.getFile(fileUploadType);
    if (!file){
      return "No file selected..."
    }
    
    return file.name;
  }

  public openFileUpload(fileUploadType: ParcelAllocationTypeStatic){
    this.fileUploads.filter(x => x.nativeElement.id == this.lowerCaseFirstLetterNoSpaces(fileUploadType.toString()) + "FileUpload")[0].nativeElement.click();
  }

  public updateParcelAllocationHistoryGrid(): void {
    this.parcelService.getParcelAllocationHistory().subscribe(result => {
      this.allocationHistoryEntries = result;
      this.updateAllocationSetDateDisplayVariables();
      this.parcelAllocationHistoryGrid ? this.parcelAllocationHistoryGrid.api.setRowData(result) : null;
    });
  }

  public clearInputs() {
    this.projectWaterAllocation.nativeElement.value = null;
    this.nativeYieldAllocation.nativeElement.value = null;
    this.fileUploads.forEach(element => {
      element.nativeElement.value = null;
      element.nativeElement.files = null;
    })
  }

  public chooseErrorToDisplay(allocationType: ParcelAllocationTypeStatic) {
    this.displayErrors[allocationType.toString()] = true;
  }

  public turnOffErrors() {
    Object.keys(this.displayErrors).forEach(key => {
      this.displayErrors[key] = false;
    });

    Object.keys(this.displayFileErrors).forEach(key => {
      this.displayFileErrors[key] = false;
    })
  }

  public updateAllocationSetDateDisplayVariables() {
    if (this.allocationHistoryEntries)
    {
      let datePipe = this.datePipe;
      this.lastProjectWaterSetDate = datePipe.transform(this.allocationHistoryEntries.filter(x => x.Allocation == "Project Water" && x.WaterYear == this.waterYearToDisplay).reduce((x, y) => x.Date > y.Date ? x : y, { Date: null }).Date, "M/d/yyyy");
      this.lastNativeYieldSetDate = datePipe.transform(this.allocationHistoryEntries.filter(x => x.Allocation == "Native Yield" && x.WaterYear == this.waterYearToDisplay).reduce((x, y) => x.Date > y.Date ? x : y, { Date: null }).Date, "M/dd/yyyy");
      this.lastReconciliationFileUploadDate = datePipe.transform(this.allocationHistoryEntries.filter(x => x.Allocation == "Reconciliation" && x.WaterYear == this.waterYearToDisplay).reduce((x, y) => x.Date > y.Date ? x : y, { Date: null }).Date, "M/dd/yyyy");
    }
  }

  public lowerCaseFirstLetterNoSpaces(lowerFirst: string):string {
    return (lowerFirst.charAt(0).toLowerCase() + lowerFirst.slice(1)).replace(" ", "");
  }
}
