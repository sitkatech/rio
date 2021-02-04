import { DOCUMENT } from '@angular/common';
import { ChangeDetectorRef, Component, Inject, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { ColDef } from 'ag-grid-community';
import { forkJoin } from 'rxjs';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterYearService } from 'src/app/services/water-year.service';
import { UserDto } from 'src/app/shared/models';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { FeatureClassInfoDto } from 'src/app/shared/models/feature-class-info-dto';
import { ParcelUpdateExpectedResultsDto } from 'src/app/shared/models/parcel-update-expected-results-dto';
import { WaterYearDto } from 'src/app/shared/models/water-year-dto';
import { ApiService } from 'src/app/shared/services';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'rio-parcel-update-layer',
  templateUrl: './parcel-update-layer.component.html',
  styleUrls: ['./parcel-update-layer.component.scss']
})
export class ParcelUpdateLayerComponent implements OnInit {

  @ViewChildren('fileInput') public fileInput: QueryList<any>;
  private watchUserChangeSubscription: any;
  public modalReference: NgbModalRef;
  public richTextTypeID: number = CustomRichTextType.ParcelUpdateLayer;


  public isLoadingSubmit: boolean;
  public rowData = [];
  public columnDefs: ColDef[];
  public allowableFileTypes = ["gdb"];
  public maximumFileSizeMB = 30;
  public newParcelLayerForm = new FormGroup({
    gdbUploadForParcelLayer: new FormControl('', [Validators.required])
  });
  public submitForPreviewForm = new FormGroup({
    waterYearSelection: new FormControl('', [Validators.required])
  })
  public gdbInputFile: any = null;
  public featureClass: FeatureClassInfoDto;
  public uploadedGdbID: number;
  public requiredColumnMappings: Array<ParcelRequiredColumnAndMappingDto> = [];
  public resultsPreview: ParcelUpdateExpectedResultsDto;
  public currentWaterYear: WaterYearDto;
  public previousWaterYear: WaterYearDto;
  waterYearsNotPresentError: boolean;
  public currentYear: number = new Date().getFullYear();
  public previousYear: number = this.currentYear - 1;

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private modalService: NgbModal,
    private apiService: ApiService,
    @Inject(DOCUMENT) private document: Document
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      forkJoin([
      this.parcelService.getParcelGDBCommonMappingToParcelStagingColumn(),
      this.waterYearService.getWaterYearForCurrentYearAndVariableYearsBack(1)]).subscribe(([model, waterYears]) => {
        if (waterYears.length < 2) {
          this.waterYearsNotPresentError = true;
          return;
        }
        this.currentWaterYear = waterYears[0];
        this.previousWaterYear = waterYears[1];
        Object.keys(model).forEach(x => {
          if (typeof model[x] === "string") {
            this.requiredColumnMappings.push(new ParcelRequiredColumnAndMappingDto({ RequiredColumnName: model[x], CommonName: x }));
          }
        })
      });
    });
  }

  get f() {
    return this.newParcelLayerForm.controls;
  }

  get submitForPreviewFormControls() {
    return this.submitForPreviewForm.controls;
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getSelectedFile(event: any) {
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      //returns bytes, but I'd rather not handle a constant that's a huge value
      return event.target.files.item(0);
    }
    return null;
  }

  public onGDBFileChange(event: any) {
    this.gdbInputFile = this.getSelectedFile(event);
    this.newParcelLayerForm.get('gdbUploadForParcelLayer').setValue(this.gdbInputFile);
  }

  public mappedColumnNameSetForRequiredColumn(requiredColumn: ParcelRequiredColumnAndMappingDto) {
    return requiredColumn.MappedColumnName !== null && requiredColumn.MappedColumnName !== undefined;
  }

  public getInputFileForGDB() {
    return this.gdbInputFile ? this.gdbInputFile.name : "No file chosen...";
  }

  public removeFeatureClasses() {
    this.featureClass = null;
  }

  public getColumns(): Array<string> {
    if (!this.featureClass) {
      return [];
    }

    return this.featureClass.Columns;
  }

  public onSubmitGDB() {
    if (!this.newParcelLayerForm.valid) {
      Object.keys(this.newParcelLayerForm.controls).forEach(field => {
        const control = this.newParcelLayerForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
      return;
    }

    this.isLoadingSubmit = true;
    this.parcelService.uploadGDB(this.gdbInputFile).subscribe(response => {
      this.isLoadingSubmit = false;
      this.uploadedGdbID = response.UploadedGdbID;
      this.featureClass = response.FeatureClasses[0];
      this.requiredColumnMappings.forEach(x => {
        x.MappedColumnName = this.featureClass.Columns.includes(x.RequiredColumnName) ? x.RequiredColumnName : undefined;
      })
    }, (error) => {
      this.alertService.pushAlert(new Alert("Failed to upload GDB! If available, error details are below.", AlertContext.Danger));
      this.apiService.sendErrorToHandleError(error);
      this.isLoadingSubmit = false;
    });

  }

  public onSubmitForPreview() {
    if (!this.submitForPreviewForm.valid) {
      Object.keys(this.submitForPreviewForm.controls).forEach(field => {
        const control = this.submitForPreviewForm.get(field);
        control.markAsTouched({ onlySelf: true });
      });
      return;
    }

    this.isLoadingSubmit = true;
    let parcelLayerUpdateDto = new ParcelLayerUpdateDto({
      ParcelLayerNameInGDB : this.featureClass.LayerName,
      UploadedGDBID : this.uploadedGdbID,
      ColumnMappings : this.requiredColumnMappings,
      YearChangesToTakeEffect : this.submitForPreviewForm.get('waterYearSelection').value
    });
    this.parcelService.getGDBPreview(parcelLayerUpdateDto).subscribe(response => {
      this.isLoadingSubmit = false;
      this.resultsPreview = response;
    }, () => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("Failed to generate preview of changes!", AlertContext.Danger));
    })
  }

  public hasCurrentYearBeenUpdated() : boolean {
    return this.currentWaterYear.ParcelLayerUpdateDate != null && this.currentWaterYear.ParcelLayerUpdateDate != undefined
  }

  public onSubmitChanges() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.isLoadingSubmit = true;
    this.parcelService.enactGDBChanges(this.submitForPreviewForm.get('waterYearSelection').value).subscribe(() => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("The update was successful", AlertContext.Success));
      this.resetWorkflow();
    }, () => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("Failed enact GDB changes!", AlertContext.Danger));
    })
  }

  public resetWorkflow() {
    this.removeResultPreview();
    this.removeFeatureClasses();
    this.gdbInputFile = null;
    this.newParcelLayerForm.reset();
    this.submitForPreviewForm.reset();
    window.scrollTo(0, 0);

    this.waterYearService.getWaterYearForCurrentYearAndVariableYearsBack(1).subscribe(waterYears => {
      if (waterYears.length < 2) {
        this.waterYearsNotPresentError = true;
        return;
      }
      this.currentWaterYear = waterYears[0];
      this.previousWaterYear = waterYears[1];
    });
  }

  public previewFormValid(): boolean {
    return this.requiredColumnMappings.every(x => x.MappedColumnName !== null && x.MappedColumnName !== undefined) && this.submitForPreviewForm.valid
  }

  public clickFileInput() {
    this.document.getElementById("gdbUploadForParcelLayer").click();
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', backdrop: 'static', keyboard: false });
  }

  public removeResultPreview() {
    this.resultsPreview = null;
  }

}

export class ParcelRequiredColumnAndMappingDto {
  RequiredColumnName: string;
  MappedColumnName: string;
  CommonName: string;

  constructor(obj?: any) {
    Object.assign(this, obj);
  }
}

export class ParcelLayerUpdateDto {
  ParcelLayerNameInGDB: string;
  UploadedGDBID: number;
  ColumnMappings: Array<ParcelRequiredColumnAndMappingDto>;
  YearChangesToTakeEffect: number;

  constructor(obj?: any) {
    Object.assign(this, obj);
  }
}

