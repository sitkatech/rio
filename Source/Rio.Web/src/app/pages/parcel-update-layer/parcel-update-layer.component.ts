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
import { UserDetailedDto } from 'src/app/shared/models/user/user-detailed-dto';
import { WaterYearDto } from 'src/app/shared/models/water-year-dto';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'rio-parcel-update-layer',
  templateUrl: './parcel-update-layer.component.html',
  styleUrls: ['./parcel-update-layer.component.scss']
})
export class ParcelUpdateLayerComponent implements OnInit {

  @ViewChildren('fileInput') public fileInput: QueryList<any>;
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
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
  public gdbInputFile: any = null;
  public featureClass: FeatureClassInfoDto;
  public uploadedGdbID: number;
  public requiredColumnMappings: Array<ParcelRequiredColumnAndMappingDto> = [];
  public resultsPreview: ParcelUpdateExpectedResultsDto;
  public mostRecentWaterYear: WaterYearDto;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef,
    private formBuilder: FormBuilder,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private modalService: NgbModal,
    @Inject(DOCUMENT) private document: Document
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      forkJoin([
      this.parcelService.getParcelGDBCommonMappingToParcelStagingColumn(),
      this.waterYearService.getDefaultWaterYearToDisplay()]).subscribe(([model, waterYear]) => {
        this.mostRecentWaterYear = waterYear;
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

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getSelectedFile(event: any) {
    const reader = new FileReader();
    if (event.target.files && event.target.files.length) {
      const [file] = event.target.files;
      let incorrectFileType = !this.allowableFileTypes.some(function (x) {
        return `${x}` == file.type;
      });
      //returns bytes, but I'd rather not handle a constant that's a huge value
      let exceedsMaximumSize = (file.size / 1024 / 1024) > this.maximumFileSizeMB;
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
    }, error => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("Failed to upload GDB!", AlertContext.Danger));
    });

  }

  public onSubmitForPreview() {
    this.isLoadingSubmit = true;
    let parcelLayerUpdateDto = new ParcelLayerUpdateDto({
      ParcelLayerNameInGDB : this.featureClass.LayerName,
      UploadedGDBID : this.uploadedGdbID,
      ColumnMappings : this.requiredColumnMappings
    });
    this.parcelService.getGDBPreview(parcelLayerUpdateDto).subscribe(response => {
      this.isLoadingSubmit = false;
      this.resultsPreview = response;
    }, error => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("Failed to generate preview of changes!", AlertContext.Danger));
    })
  }

  public onSubmitChanges() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    this.parcelService.enactGDBChanges().subscribe(response => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("The update was successful", AlertContext.Success));
    }, error => {
      this.isLoadingSubmit = false;
      this.alertService.pushAlert(new Alert("Failed enact GDB changes!", AlertContext.Danger));
    })
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

  constructor(obj?: any) {
    Object.assign(this, obj);
  }
}

