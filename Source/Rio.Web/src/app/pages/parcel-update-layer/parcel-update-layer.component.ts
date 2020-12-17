import { ChangeDetectorRef, Component, OnInit, QueryList, ViewChildren } from '@angular/core';
import { FormGroup, FormBuilder, FormControl, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { UserDto } from 'src/app/shared/models';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { FeatureClassInfoDto } from 'src/app/shared/models/feature-class-info-dto';
import { UserDetailedDto } from 'src/app/shared/models/user/user-detailed-dto';
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

  public isLoadingSubmit: boolean;
  public rowData = [];
  public columnDefs: ColDef[];
  public allowableFileTypes = ["gdb"];
  public maximumFileSizeMB = 30;
  public newParcelLayerForm = new FormGroup({
    gdbUploadForParcelLayer: new FormControl ('', [Validators.required])
  });
  public gdbInputFile: any = null;
  public featureClass: FeatureClassInfoDto;
  public uploadedGdbID: number;
  public requiredColumnNames: Array<string> = ["APN_LABEL", "ASSE_NAME"];
  public requiredColumnMappings: Array<ParcelRequiredColumnAndMappingDto> = [];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private cdr: ChangeDetectorRef,
    private formBuilder: FormBuilder,
    private parcelService: ParcelService
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.requiredColumnNames.forEach(x => {
        this.requiredColumnMappings.push(new ParcelRequiredColumnAndMappingDto({RequiredColumnName:x}));
      });
    });
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
  }

  public mappedColumnNameSetForRequiredColumn(requiredColumn : ParcelRequiredColumnAndMappingDto) {
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
      this.isLoadingSubmit = true;
      this.parcelService.uploadGDB(this.gdbInputFile).subscribe(response => {
        this.isLoadingSubmit = false;
        this.uploadedGdbID = response.UploadedGdbID;
        this.featureClass = response.FeatureClasses[0];
        this.requiredColumnMappings.forEach(x => {
          x.MappedColumnName = this.featureClass.Columns.includes(x.RequiredColumnName) ? x.RequiredColumnName : undefined;
        })
      }, error => {
        console.log(error);
        const taskResponse = error.error;
        this.alertService.pushAlert(new Alert("Failed to upload GDB!  Reason: " + (typeof taskResponse === 'string' ? taskResponse : taskResponse.Reason), AlertContext.Danger));
      });

  }

  public onSubmit() {
    console.log("Not implemented");
  }

}

export class ParcelRequiredColumnAndMappingDto {
  RequiredColumnName : string;
  MappedColumnName : string;

  constructor(obj?: any){
    Object.assign(this, obj);
}
}
