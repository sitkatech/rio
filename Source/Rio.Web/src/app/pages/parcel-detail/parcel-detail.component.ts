import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { isNullOrUndefined } from 'util';
import { UserDto } from 'src/app/shared/models';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public parcel: ParcelDto;
  public parcelAllocationAndConsumptions: Array<ParcelAllocationAndConsumptionDto>;
  public months: number[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;

      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelAllocationAndConsumption(id)
        ).subscribe(([parcel, parcelAllocationAndConsumptions]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcelAllocationAndConsumptions = parcelAllocationAndConsumptions.sort((a, b) => a.WaterYear > b.WaterYear ? -1 : a.WaterYear < b.WaterYear ? 1 : 0);
          this.cdr.detectChanges();
        });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }

  public getConsumptionForMonth(parcelAllocationAndConsumption: ParcelAllocationAndConsumptionDto, month: number): string {
    var monthlyEvapotranspiration = parcelAllocationAndConsumption.MonthlyEvapotranspiration.find(x => x.WaterMonth == month);
    return isNullOrUndefined(monthlyEvapotranspiration) ? "-" : monthlyEvapotranspiration.EvapotranspirationRate.toFixed(1);
  }

  public getConsumptionForYear(parcelAllocationAndConsumption: ParcelAllocationAndConsumptionDto): string {
    var monthlyEvapotranspiration = parcelAllocationAndConsumption.MonthlyEvapotranspiration;
    if (monthlyEvapotranspiration.length > 0) {
      let result = monthlyEvapotranspiration.reduce(function (a, b) {
        return (a + b.EvapotranspirationRate);
      }, 0);
      return result.toFixed(1);
    }
    else {
      return "-";
    }
  }
}
