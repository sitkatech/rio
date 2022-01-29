import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { WaterTypeApplicationTypeEnum } from 'src/app/shared/models/water-type-application-type-enum';
import { Router } from '@angular/router';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';

@Component({
  selector: 'rio-water-type-edit',
  templateUrl: './water-type-edit.component.html',
  styleUrls: ['./water-type-edit.component.scss']
})
export class WaterTypeEditComponent implements OnInit, OnDestroy {
  watchUserChangeSubscription: any;
  existingWaterTypes: WaterTypeDto[];
  waterTypes: WaterTypeDto[] = [];
  isLoadingSubmit: boolean = false;
  modalReference: NgbModalRef;
  richTextTypeID: number = CustomRichTextType.ConfigureWaterTypes;
  @ViewChild("deleteWarningModalContent") deleteWarningModalContent

  WaterTypeApplicationTypeEnum = WaterTypeApplicationTypeEnum;

  constructor(
    private router: Router,
    private alertService: AlertService,
    private waterTypeService: WaterTypeService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.resetWaterTypes();
    })
  }

  ngOnDestroy(): void {
    
  }

  resetWaterTypes(): void {
    this.isLoadingSubmit = true;
    this.waterTypeService.getWaterTypes().subscribe(waterTypes => {
      this.isLoadingSubmit = false;
      this.waterTypes = waterTypes;
      this.cdr.detectChanges();
    }, error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    })
  }

  deleteWaterType(waterType: WaterTypeDto): void {
    const index = this.waterTypes.indexOf(waterType);
    this.waterTypes.splice(index, 1);
  }

  addWaterType(): void {
    this.waterTypes.push(new WaterTypeDto());
  }

  onSubmit(form: any) {
    this.launchModal(this.deleteWarningModalContent)
  }

  submitImpl(): void {
    this.isLoadingSubmit = true;
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.waterTypeService.mergeWaterTypes(this.waterTypes).subscribe(x => {
      this.isLoadingSubmit = false;
      this.router.navigateByUrl("/manager-dashboard").then(x => {
        this.alertService.pushAlert(new Alert("Water types successfully updated.", AlertContext.Success));
      });
    }, error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    })
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', ariaLabelledBy: 'deleteWarningModalTitle', backdrop: 'static', keyboard: false });


  }
}
