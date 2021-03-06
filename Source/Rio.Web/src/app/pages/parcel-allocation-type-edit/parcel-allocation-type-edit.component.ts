import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild } from '@angular/core';
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelAllocationTypeDto, ParcelAllocationTypeApplicationTypeEnum } from 'src/app/shared/models/parcel-allocation-type-dto';
import { Router } from '@angular/router';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextComponent } from 'src/app/shared/components/custom-rich-text/custom-rich-text.component';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-parcel-allocation-type-edit',
  templateUrl: './parcel-allocation-type-edit.component.html',
  styleUrls: ['./parcel-allocation-type-edit.component.scss']
})
export class ParcelAllocationTypeEditComponent implements OnInit, OnDestroy {
  watchUserChangeSubscription: any;
  existingParcelAllocationTypes: ParcelAllocationTypeDto[];
  parcelAllocationTypes: ParcelAllocationTypeDto[] = [];
  isLoadingSubmit: boolean = false;
  modalReference: NgbModalRef;
  richTextTypeID: number = CustomRichTextType.ConfigureWaterTypes;
  @ViewChild("deleteWarningModalContent") deleteWarningModalContent

  ParcelAllocationTypeApplicationTypeEnum = ParcelAllocationTypeApplicationTypeEnum;

  constructor(
    private router: Router,
    private alertService: AlertService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private modalService: NgbModal
  ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.resetParcelAllocationTypes();
    })
  }

  ngOnDestroy(): void {
    this.watchUserChangeSubscription.unsubscribe();
  }

  resetParcelAllocationTypes(): void {
    this.isLoadingSubmit = true;
    this.parcelAllocationTypeService.getParcelAllocationTypes().subscribe(parcelAllocationTypes => {
      this.isLoadingSubmit = false;
      this.parcelAllocationTypes = parcelAllocationTypes;
      this.cdr.detectChanges();
    }, error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    })
  }

  deleteParcelAllocationType(parcelAllocationType: ParcelAllocationTypeDto): void {
    const index = this.parcelAllocationTypes.indexOf(parcelAllocationType);
    this.parcelAllocationTypes.splice(index, 1);
  }

  addParcelAllocationType(): void {
    this.parcelAllocationTypes.push(new ParcelAllocationTypeDto());
  }

  onSubmit(form: any) {
    this.launchModal(this.deleteWarningModalContent)
  }

  isSourcedFromApiValid():boolean{
    const sourcedfromApiCount = this.parcelAllocationTypes.filter(x=>x.IsAppliedProportionally ==  ParcelAllocationTypeApplicationTypeEnum.Api).length ;
    return sourcedfromApiCount <= 1;
  }

  submitImpl(): void {
    this.isLoadingSubmit = true;
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.parcelAllocationTypeService.mergeParcelAllocationTypes(this.parcelAllocationTypes).subscribe(x => {
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
