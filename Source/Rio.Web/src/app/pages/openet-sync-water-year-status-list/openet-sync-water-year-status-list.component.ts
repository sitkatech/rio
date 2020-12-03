import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin } from 'rxjs';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OpenETService } from 'src/app/services/openet.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterYearService } from 'src/app/services/water-year.service';
import { UserDto } from 'src/app/shared/models';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { OpenETSyncHistoryDto, WaterYearDto } from 'src/app/shared/models/openet-sync-history-dto';
import { OpenETSyncResultTypeEnum } from 'src/app/shared/models/openet-sync-result-type-dto';
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

  public waterYears: Array<WaterYearDto>;
  public inProgressSyncDtos: Array<OpenETSyncHistoryDto>;
  public isPerformingAction: boolean = false;

  public dateFormatString: string = "M/dd/yyyy";
  selectedWaterYear: WaterYearDto;
  openETSyncHistoryDtos: Array<OpenETSyncHistoryDto>;

  constructor(
    private authenticationService: AuthenticationService,
    private openETService: OpenETService,
    private waterYearService: WaterYearService,
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
      this.waterYearService.getWaterYears(),
      this.openETService.getOpenETSyncHistory()
    ]).subscribe(([waterYears, openETSyncHistories]) => {
      this.isPerformingAction = false;
      this.waterYears = waterYears;
      this.openETSyncHistoryDtos = openETSyncHistories;
    });
  }

  public isCurrentUserAdministrator(): boolean {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public getDataUpdateStatusForWaterYear(waterYear: WaterYearDto): string {
    if (waterYear.FinalizeDate !== null) {
      return "Finalized " + this.datePipe.transform(waterYear.FinalizeDate, this.dateFormatString);
    }

    if (waterYear.Year > new Date().getFullYear()) {
      return "Data Not Yet Available";
    }

    if (this.openETSyncHistoryDtos && this.openETSyncHistoryDtos.some(x => x.WaterYear.WaterYearID == waterYear.WaterYearID && x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress)) {
      return "Update In Progress";
    }

    return "Checking For Updates Nightly";
  }

  public updatingWaterYears() : Array<OpenETSyncHistoryDto> {
    if (!this.openETSyncHistoryDtos) {
      return null;
    }

    return this.openETSyncHistoryDtos.filter(x => x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress);
  }

  public waterYearIsUpdating(waterYear: WaterYearDto): boolean {
    if (!this.openETSyncHistoryDtos) {
      return false;
    }

    return this.openETSyncHistoryDtos.some(x => x.WaterYear.WaterYearID == waterYear.WaterYearID && x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress);
  }

  public getLastUpdatedDateForWaterYear(waterYear: WaterYearDto): string {
    if (waterYear.Year > new Date().getFullYear()) {
      return "-";
    }
debugger;
    let successfulUpdates = this.openETSyncHistoryDtos.filter(x => x.WaterYear.WaterYearID == waterYear.WaterYearID && x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.Succeeded);

    return successfulUpdates.length > 0 ? this.datePipe.transform(successfulUpdates.reduce((mostRecentDate, syncHistoryDto) => mostRecentDate > syncHistoryDto.UpdateDate ? mostRecentDate : syncHistoryDto.UpdateDate,  new Date('0001-01-01T00:00:00Z')) , this.dateFormatString) : "Has not been updated";
  }

  public showActionButtonsForWaterYear(waterYear: WaterYearDto): boolean {
    return waterYear.Year <= new Date().getFullYear();
  }

  public showFinalizeButton(waterYear: WaterYearDto): boolean {
    if (waterYear.Year >= new Date().getFullYear()) {
      return false;
    }

    return waterYear.FinalizeDate == null;
  }

  public setSelectedWaterYearAndLaunchModal(modalContent: any, waterYear: WaterYearDto) {
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
    this.openETService.triggerGoogleBucketRefreshForWaterYear(this.selectedWaterYear.WaterYearID).subscribe(response => {
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
    this.waterYearService.finalizeWaterYear(this.selectedWaterYear.WaterYearID).subscribe(response => {
      this.alertService.pushAlert(new Alert(`The Evapotranspiration Data for ${this.selectedWaterYear} was successfully finalized`, AlertContext.Success));
      this.selectedWaterYear = null;
      this.refreshWaterYearsAndOpenETSyncData();
    })
  }

  public getInProgressYears(): string {
    if (!this.openETSyncHistoryDtos || this.openETSyncHistoryDtos.length == 0) {
      return "";
    }

    let allInProgressOperations = this.openETSyncHistoryDtos.filter(x => x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress);

    if (allInProgressOperations.length == 0) {
      return "";
    }

    var allYearsInProgressUniqueInString = allInProgressOperations.map(x => x.WaterYear.Year).sort().join(", ");

    return allYearsInProgressUniqueInString;
  }
}
