import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ActivatedRoute, Router } from '@angular/router';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { OverconsumptionRateUpsertDto } from 'src/app/shared/generated/model/overconsumption-rate-upsert-dto';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-overconsumption-rate-edit',
  templateUrl: './overconsumption-rate-edit.component.html',
  styleUrls: ['./overconsumption-rate-edit.component.scss']
})
export class OverconsumptionRateEditComponent implements OnInit, OnDestroy {
  private currentUser: UserDto;
  public watchAccountChangeSubscription: any;

  public waterYears: WaterYearDto[];
  public model = new OverconsumptionRateUpsertDto();
  public currentOverconsumptionRate: number;

  public customRichTextTypeID = CustomRichTextTypeEnum.SetOverconsumptionRate;
  public isLoadingSubmit: boolean = false;

  constructor(
    private router: Router,
    private alertService: AlertService,
    private authenticationService: AuthenticationService,
    private waterYearService: WaterYearService,
    private cdr: ChangeDetectorRef
  ) { }

  ngOnInit() {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.waterYearService.getWaterYears().subscribe(waterYears => {
        this.waterYears = waterYears;
      });
    });
  }

  onWaterYearSelected() {
    const i = this.waterYears.findIndex(x => x.WaterYearID == this.model.WaterYearID);
    this.currentOverconsumptionRate = this.waterYears[i]?.OverconsumptionRate;
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  public onSubmit() {
    this.isLoadingSubmit = true;
    this.waterYearService.updateOverconsumptionRate(this.model.WaterYearID, this.model).subscribe(() => {
      this.isLoadingSubmit = false;
      this.router.navigateByUrl(`/manager-dashboard`).then(x => {
        this.alertService.pushAlert(new Alert(`Overconsumption rate successfully updated`, AlertContext.Success));
      });
    }), error => {
      this.isLoadingSubmit = false;
      this.cdr.detectChanges();
    };
  }
}
