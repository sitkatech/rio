import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin } from 'rxjs';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OpenETService } from 'src/app/services/openet.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { UserDto } from 'src/app/shared/models';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { OpenETSyncHistoryDto } from 'src/app/shared/models/openet-sync-history-dto';
import { OpenETSyncStatusTypeEnum } from 'src/app/shared/models/openet-sync-status-type-dto';
import { OpenETSyncWaterYearStatusDto } from 'src/app/shared/models/openet-sync-water-year-status-dto';
import { AlertService } from 'src/app/shared/services/alert.service';

@Component({
  selector: 'rio-openet-sync-water-year-status-list',
  templateUrl: './openet-sync-water-year-status-list.component.html',
  styleUrls: ['./openet-sync-water-year-status-list.component.scss']
})
export class OpenetSyncWaterYearStatusListComponent implements OnInit {
  watchUserChangeSubscription: any;
  currentUser: UserDto;
  richTextTypeID: number = CustomRichTextType.OpenETIntegration;
  public modalReference: NgbModalRef;

  public waterYears: Array<number>;
  public openETSyncWaterYearStatusDtos: Array<OpenETSyncWaterYearStatusDto>;
  public inProgressSyncDtos: Array<OpenETSyncHistoryDto>;
  public isPerformingAction: boolean = false;

  public dateFormatString: string = "M/dd/yyyy";
  selectedWaterYear: number;

  constructor(
    private authenticationService: AuthenticationService,
    private openETService: OpenETService,
    private parcelService: ParcelService,
    private datePipe: DatePipe,
    private modalService: NgbModal,
    private alertService: AlertService
  ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.refreshWaterYearsAndOpenETSyncData();     
    });
  }

  private refreshWaterYearsAndOpenETSyncData() {
    this.isPerformingAction = true;
    forkJoin([
      this.parcelService.getWaterYears(),
      this.openETService.listAllOpenETSyncWaterYearStatus(),
      this.openETService.getInProgressOpenETSyncHistory()
    ]).subscribe(([waterYears, openETSyncWaterYearStatusDtos, inProgressSync]) => {
      this.isPerformingAction = false;
      this.waterYears = waterYears;
      this.openETSyncWaterYearStatusDtos = openETSyncWaterYearStatusDtos;
      this.inProgressSyncDtos = inProgressSync;
    });
  }

  public isCurrentUserAdministrator() : boolean {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public getDataUpdateStatusForWaterYear(waterYear : number) : string {
    if (!this.openETSyncWaterYearStatusDtos || !this.openETSyncWaterYearStatusDtos.some(x => x.WaterYear == waterYear)) {
      return "Data not yet available";
    }

    let dtoForWaterYear = this.openETSyncWaterYearStatusDtos.filter(x => x.WaterYear == waterYear)[0];

    switch(dtoForWaterYear.OpenETSyncStatusType.OpenETSyncStatusTypeID)
    {
      case OpenETSyncStatusTypeEnum.Nightly:
        return "Checking for Updates Nightly";
      case OpenETSyncStatusTypeEnum.Finalized:
        return "Finalized " + this.datePipe.transform(dtoForWaterYear.LastUpdatedDate, this.dateFormatString);
      case OpenETSyncStatusTypeEnum.CurrentlyUpdating:
        return "Update in Progress";
      default:
        return "Status type not recognized"
    }
  }

  public waterYearIsUpdating(waterYear:number) : boolean {
    if (!this.openETSyncWaterYearStatusDtos || !this.openETSyncWaterYearStatusDtos.some(x => x.WaterYear == waterYear)) {
      return false;
    } 

    return this.openETSyncWaterYearStatusDtos.filter(x => x.WaterYear == waterYear)[0].OpenETSyncStatusType.OpenETSyncStatusTypeID == OpenETSyncStatusTypeEnum.CurrentlyUpdating;
  }

  public getLastUpdatedDateForWaterYear(waterYear:number) : string {
    if (!this.openETSyncWaterYearStatusDtos || !this.openETSyncWaterYearStatusDtos.some(x => x.WaterYear == waterYear)) {
      return "-";
    }

    let lastUpdatedDate = this.openETSyncWaterYearStatusDtos.filter(x => x.WaterYear == waterYear)[0].LastUpdatedDate;

    return lastUpdatedDate ? this.datePipe.transform(lastUpdatedDate, this.dateFormatString) : "Has not been updated";
  }

  public showActionButtonsForWaterYear(waterYear:number) : boolean {
    if (!this.openETSyncWaterYearStatusDtos || !this.openETSyncWaterYearStatusDtos.some(x => x.WaterYear == waterYear)) {
      return false;
    } 

    return this.openETSyncWaterYearStatusDtos.filter(x => x.WaterYear == waterYear)[0].OpenETSyncStatusType.OpenETSyncStatusTypeID != OpenETSyncStatusTypeEnum.Finalized;
  }

  public setSelectedWaterYearAndLaunchModal(modalContent: any, waterYear: number) {
    this.selectedWaterYear = waterYear;
    this.launchModal(modalContent);
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', backdrop: 'static', keyboard: false });
  }

  public resetWaterYearToFinalizeAndCloseModal(modal: any) {
    this.selectedWaterYear = null;
    this.closeModal(modal, "Cancel click")
  }

  public closeModal(modal: any, reason: string) {
    modal.close(reason);
  }

  public syncWaterYear() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.isPerformingAction = true;
    this.openETService.triggerGoogleBucketRefreshForWaterYear(this.selectedWaterYear).subscribe(response => {
      this.alertService.pushAlert(new Alert(`The request to sync data for ${this.selectedWaterYear} was successfully submitted. Please allow for at least 15 minutes for the update to complete and take effect.`, AlertContext.Success));
      this.selectedWaterYear = null;
      this.refreshWaterYearsAndOpenETSyncData();
    })
  }

  public finalizeWaterYear() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.isPerformingAction = true;
    let openETSyncWaterYearStatusIDForWaterYear = this.openETSyncWaterYearStatusDtos.filter(x => x.WaterYear == this.selectedWaterYear)[0].OpenETSyncWaterYearStatusID;
    this.openETService.finalizeOpenETSyncWaterYearStatus(openETSyncWaterYearStatusIDForWaterYear).subscribe(response => {
      this.alertService.pushAlert(new Alert(`The Evapotranspiration Data for ${this.selectedWaterYear} was successfully finalized`, AlertContext.Success));
      this.selectedWaterYear = null;
      this.refreshWaterYearsAndOpenETSyncData();
    })
  }

  public getInProgressYears() : string {
    if (!this.inProgressSyncDtos) {
      return "";
    } 

    if (this.inProgressSyncDtos.length == 1) {
      return this.inProgressSyncDtos[0].YearsInUpdateSeparatedByComma;
    }

    var allYearsInProgress = this.inProgressSyncDtos.map(x => {
      return x.YearsInUpdateSeparatedByComma.split(",");
    });

    var allYearsInProgressUniqueInString = allYearsInProgress.filter((n, i) => allYearsInProgress.indexOf(n) === i).sort().join(", ");

    return allYearsInProgressUniqueInString;
  
  }
}
