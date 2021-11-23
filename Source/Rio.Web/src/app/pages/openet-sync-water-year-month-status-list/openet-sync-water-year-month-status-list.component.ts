import { DatePipe } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { forkJoin } from 'rxjs';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { OpenETService } from 'src/app/services/openet.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { environment } from 'src/environments/environment';
import { WaterYearMonthService } from 'src/app/services/water-year-month.service';
import { finalize } from 'rxjs/operators';
import { OpenETSyncResultTypeEnum } from 'src/app/shared/models/enums/open-et-sync-result-type-enum';
import { OpenETSyncHistoryDto } from 'src/app/shared/generated/model/open-et-sync-history-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterYearMonthDto } from 'src/app/shared/generated/model/water-year-month-dto';

@Component({
  selector: 'rio-openet-sync-water-year-month-status-list',
  templateUrl: './openet-sync-water-year-month-status-list.component.html',
  styleUrls: ['./openet-sync-water-year-month-status-list.component.scss']
})
export class OpenetSyncWaterYearMonthStatusListComponent implements OnInit {
  public watchUserChangeSubscription: any;
  public currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextType.OpenETIntegration;
  public modalReference: NgbModalRef;

  public waterYearMonthDtos: Array<WaterYearMonthDto>;
  public inProgressSyncDtos: Array<OpenETSyncHistoryDto>;
  public isPerformingAction: boolean = false;

  public dateFormatString: string = "M/dd/yyyy hh:mm a";
  public monthNameFormatter: any = new Intl.DateTimeFormat('en-us', { month: 'long' });
  public selectedWaterYearMonth: WaterYearMonthDto;
  public selectedWaterYearMonthName: string;
  public mostRecentSyncHistoryDtos: Array<OpenETSyncHistoryDto>;
  public isOpenETAPIKeyValid: boolean;
  public loadingPage: boolean = true;
  public syncsInProgress: OpenETSyncHistoryDto[];

  constructor(
    private authenticationService: AuthenticationService,
    private openETService: OpenETService,
    private waterYearMonthService: WaterYearMonthService,
    private datePipe: DatePipe,
    private modalService: NgbModal,
    private alertService: AlertService
  ) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.loadingPage = true;
      this.currentUser = currentUser;
      this.openETService.isApiKeyValid().subscribe(isValid => {
        this.isOpenETAPIKeyValid = isValid;
        this.refreshWaterYearMonthsAndOpenETSyncData();
        this.loadingPage = false;
      })
    });
  }

  private refreshWaterYearMonthsAndOpenETSyncData() {
    this.isPerformingAction = true;
    forkJoin([
      this.waterYearMonthService.getWaterYearMonthsForCurrentDateOrEarlier(),
      this.waterYearMonthService.getMostRecentSyncHistoryForWaterYearMonthsThatHaveBeenUpdated()]).subscribe(([waterYearMonths, abbreviatedWaterYearSyncHistories]) => {
        this.isPerformingAction = false;
        this.waterYearMonthDtos = waterYearMonths;
        this.mostRecentSyncHistoryDtos = abbreviatedWaterYearSyncHistories;
        this.syncsInProgress = this.mostRecentSyncHistoryDtos.filter(x => x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress);
      });
  }

  public isCurrentUserAdministrator(): boolean {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public getDataUpdateStatusForWaterYearMonth(waterYearMonth: WaterYearMonthDto): string {
    let currentDate = new Date();
    if (new Date(waterYearMonth.WaterYear.Year, waterYearMonth.Month - 1) > new Date(currentDate.getFullYear(), currentDate.getMonth())) {
      return "Data Not Yet Available";
    }

    let mostRecentSyncResult = this.mostRecentSyncHistoryDtos.filter(x => x.WaterYearMonth.WaterYearMonthID == waterYearMonth.WaterYearMonthID)[0];

    if (mostRecentSyncResult == null || mostRecentSyncResult == undefined) {
      return "Update has not yet been run";
    }

    var defaultOpeningStatement = `Queried OpenET on ${this.datePipe.transform(mostRecentSyncResult.CreateDate, this.dateFormatString)}:`;

    switch (mostRecentSyncResult.OpenETSyncResultType.OpenETSyncResultTypeID) {
      case OpenETSyncResultTypeEnum.InProgress:
        return `OpenET query in progress, started on ${this.datePipe.transform(mostRecentSyncResult.CreateDate, this.dateFormatString)}`;
      case OpenETSyncResultTypeEnum.Failed:
        return `Last OpenET query failed on ${this.datePipe.transform(mostRecentSyncResult.CreateDate, this.dateFormatString)}. Error message: ${mostRecentSyncResult.ErrorMessage}`;
      case OpenETSyncResultTypeEnum.DataNotAvailable:
        return `${defaultOpeningStatement} No data available for this month`;
      case OpenETSyncResultTypeEnum.NoNewData:
        return `${defaultOpeningStatement} No updates found`;
      case OpenETSyncResultTypeEnum.Succeeded:
        return `${defaultOpeningStatement} Updated data was successfully imported`;
      case OpenETSyncResultTypeEnum.Created:
        return `${defaultOpeningStatement} Attempting to create OpenET query request`;
      default:
        return `Unrecognized result`;
    }
  }

  public numSyncsInProgress(): number {
    if (!this.mostRecentSyncHistoryDtos) {
      return 0;
    }

    return this.mostRecentSyncHistoryDtos.filter(x => x.OpenETSyncResultType.OpenETSyncResultTypeID == OpenETSyncResultTypeEnum.InProgress).length;
  }

  public showActionButtonsForWaterYearMonth(waterYearMonth: WaterYearMonthDto): boolean {
    var currentDate = new Date();
    return (new Date(waterYearMonth.WaterYear.Year, waterYearMonth.Month - 1) <= new Date(currentDate.getFullYear(), currentDate.getMonth())) && waterYearMonth.FinalizeDate == null;
  }

  public setSelectedWaterYearMonthAndLaunchModal(modalContent: any, waterYearMonth: WaterYearMonthDto) {
    this.selectedWaterYearMonth = waterYearMonth;
    this.selectedWaterYearMonthName = this.monthNameFormatter.format(new Date(waterYearMonth.WaterYear.Year, waterYearMonth.Month - 1));
    this.launchModal(modalContent);
  }

  public launchModal(modalContent: any) {
    this.modalReference = this.modalService.open(modalContent, { windowClass: 'modal-size', backdrop: 'static', keyboard: false });
  }

  public resetWaterYearMonthToFinalizeAndCloseModal(modal: any) {
    this.selectedWaterYearMonth = null;
    this.selectedWaterYearMonthName = null;
    this.closeModal(modal, "Cancel click")
  }

  public closeModal(modal: any, reason: string) {
    modal.close(reason);
  }

  public syncWaterYearMonth() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }

    this.isPerformingAction = true;
    this.openETService.triggerGoogleBucketRefreshForWaterYearMonth(this.selectedWaterYearMonth.WaterYearMonthID).pipe(
      finalize(() => {
        this.isPerformingAction = false;
        this.selectedWaterYearMonth = null;
        this.selectedWaterYearMonthName = null;
        this.refreshWaterYearMonthsAndOpenETSyncData();
      }),
    ).subscribe(response => {
      this.alertService.pushAlert(new Alert(`The request to sync data for ${this.selectedWaterYearMonthName} ${this.selectedWaterYearMonth.WaterYear.Year} was successfully submitted. The update may take a while, but will continue in the background.`, AlertContext.Success));
    });
  }

  public openETSyncEnabled() {
    return environment.allowOpenETSync;
  }

  public finalizeWaterYearMonth() {
    if (this.modalReference) {
      this.modalReference.close();
      this.modalReference = null;
    }
    this.isPerformingAction = true;
    this.waterYearMonthService.finalizeWaterYearMonth(this.selectedWaterYearMonth.WaterYearMonthID).pipe(
      finalize(() => {
        this.isPerformingAction = false;
        this.selectedWaterYearMonth = null;
        this.selectedWaterYearMonthName = null;
        this.refreshWaterYearMonthsAndOpenETSyncData();
      }),
    ).subscribe(response => {
      this.alertService.pushAlert(new Alert(`The Evapotranspiration Data for ${this.selectedWaterYearMonthName} ${this.selectedWaterYearMonth.WaterYear.Year} was successfully finalized`, AlertContext.Success));
    })
  }

  public syncInProgressForWaterYearMonth(waterYearMonthID : number): boolean {
    return this.syncsInProgress.length > 0 && this.syncsInProgress.some(x => x.WaterYearMonth.WaterYearMonthID == waterYearMonthID);
  }

  public getInProgressDates(): string {
    if (this.syncsInProgress.length == 0) {
      return "";
    }

    var allYearsInProgressUniqueInString = this.syncsInProgress.sort((x, y) => {
      //this should technically be an error case, we should never have two updates for the same month running at the same time
      if (x.WaterYearMonth.WaterYear.Year == y.WaterYearMonth.WaterYear.Year && x.WaterYearMonth.Month == y.WaterYearMonth.Month) {
        return 0;
      }

      if (x.WaterYearMonth.WaterYear.Year > y.WaterYearMonth.WaterYear.Year ||
        (x.WaterYearMonth.WaterYear.Year == y.WaterYearMonth.WaterYear.Year && x.WaterYearMonth.Month > y.WaterYearMonth.Month)) {
        return 1;
      }

      return -1;
    }).map(x => {
      let monthName = this.monthNameFormatter.format(new Date(x.WaterYearMonth.WaterYear.Year, x.WaterYearMonth.Month - 1));
      return `${monthName} ${x.WaterYearMonth.WaterYear.Year}`
    }).join(", ");

    return allYearsInProgressUniqueInString;
  }
}
