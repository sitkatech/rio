import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { isNullOrUndefined } from 'util';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ParcelDetailComponent implements OnInit {
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
  }

  ngOnInit() {
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
          forkJoin(
              this.parcelService.getParcelByParcelID(id),
              this.parcelService.getParcelAllocationAndConsumption(id)
          ).subscribe(([parcel, parcelAllocationAndConsumptions]) => {
              this.parcel = parcel instanceof Array
                  ? null
                  : parcel as ParcelDto;
              this.parcel = parcel;
              this.parcelAllocationAndConsumptions = parcelAllocationAndConsumptions;
              this.cdr.detectChanges();
          });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12]
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }

  public getConsumptionForMonth(parcelAllocationAndConsumption: ParcelAllocationAndConsumptionDto, month: number): string {
    var a = parcelAllocationAndConsumption.MonthlyEvapotranspiration.find(x => x.WaterMonth == month);
    return isNullOrUndefined(a) ? "" : a.EvapotranspirationRate.toFixed(2);
  }
}
