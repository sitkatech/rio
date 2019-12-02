import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { Alert } from 'src/app/shared/models/alert';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelAllocationUpsertWrapperDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-wrapper-dto.';
import { UserDto } from 'src/app/shared/models';
import { ParcelAllocationDto } from 'src/app/shared/models/parcel/parcel-allocation-dto';
import { ParcelAllocationTypeEnum } from 'src/app/shared/models/enums/parcel-allocation-type-enum';

@Component({
  selector: 'rio-parcel-edit-allocation',
  templateUrl: './parcel-edit-allocation.component.html',
  styleUrls: ['./parcel-edit-allocation.component.scss']
})
export class ParcelEditAllocationComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public waterYears: Array<number>;
  public parcel: ParcelDto;
  public parcelAllocations: Array<ParcelAllocationDto>;
  public model: any;
  public isLoadingSubmit: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private alertService: AlertService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelAllocations(id),
          this.parcelService.getWaterYears()
        ).subscribe(
          ([parcel, parcelAllocations, waterYears]) => {
            this.parcel = parcel instanceof Array
              ? null
              : parcel as ParcelDto;
            this.parcel = parcel;
            this.parcelAllocations = parcelAllocations;
            this.waterYears = waterYears;
            this.model = waterYears.map(year => {
              return {
                WaterYear: year,
                ProjectWater: this.getAnnualAllocationForType(year, ParcelAllocationTypeEnum.ProjectWater),
                Reconciliation: this.getAnnualAllocationForType(year, ParcelAllocationTypeEnum.Reconciliation),
                NativeYield: this.getAnnualAllocationForType(year, ParcelAllocationTypeEnum.NativeYield),
                StoredWater: this.getAnnualAllocationForType(year, ParcelAllocationTypeEnum.StoredWater)
              };
            });
          }
        );
      }
    });
  }

  private getAnnualAllocationForType(year: number, parcelAllocationTypeEnum: ParcelAllocationTypeEnum) {
    const parcelAllocation = this.parcelAllocations.find(pa => pa.WaterYear === year && pa.ParcelAllocationTypeID === parcelAllocationTypeEnum);
    return parcelAllocation ? parcelAllocation.AcreFeetAllocated : null;
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public onSubmit(editAllocationForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    let parcelAllocationUpsertWrapperDto = new ParcelAllocationUpsertWrapperDto();
    parcelAllocationUpsertWrapperDto.ParcelAllocations = [];
    this.model.filter(pa => pa.ProjectWater > 0).forEach(pa => {
      let parcelAllocationUpsertDto = new ParcelAllocationUpsertDto();
      parcelAllocationUpsertDto.WaterYear = pa.WaterYear;
      parcelAllocationUpsertDto.ParcelAllocationTypeID = ParcelAllocationTypeEnum.ProjectWater;
      parcelAllocationUpsertDto.AcreFeetAllocated = pa.ProjectWater;
      parcelAllocationUpsertWrapperDto.ParcelAllocations.push(parcelAllocationUpsertDto);
    });
    this.model.filter(pa => pa.Reconciliation > 0).forEach(pa => {
      let parcelAllocationUpsertDto = new ParcelAllocationUpsertDto();
      parcelAllocationUpsertDto.WaterYear = pa.WaterYear;
      parcelAllocationUpsertDto.ParcelAllocationTypeID = ParcelAllocationTypeEnum.Reconciliation;
      parcelAllocationUpsertDto.AcreFeetAllocated = pa.Reconciliation;
      parcelAllocationUpsertWrapperDto.ParcelAllocations.push(parcelAllocationUpsertDto);
    });
    this.model.filter(pa => pa.NativeYield > 0).forEach(pa => {
      let parcelAllocationUpsertDto = new ParcelAllocationUpsertDto();
      parcelAllocationUpsertDto.WaterYear = pa.WaterYear;
      parcelAllocationUpsertDto.ParcelAllocationTypeID = ParcelAllocationTypeEnum.NativeYield;
      parcelAllocationUpsertDto.AcreFeetAllocated = pa.NativeYield;
      parcelAllocationUpsertWrapperDto.ParcelAllocations.push(parcelAllocationUpsertDto);
    });
    this.model.filter(pa => pa.StoredWater > 0).forEach(pa => {
      let parcelAllocationUpsertDto = new ParcelAllocationUpsertDto();
      parcelAllocationUpsertDto.WaterYear = pa.WaterYear;
      parcelAllocationUpsertDto.ParcelAllocationTypeID = ParcelAllocationTypeEnum.StoredWater;
      parcelAllocationUpsertDto.AcreFeetAllocated = pa.StoredWater;
      parcelAllocationUpsertWrapperDto.ParcelAllocations.push(parcelAllocationUpsertDto);
    });
      
    this.parcelService.updateAnnualAllocations(this.parcel.ParcelID, parcelAllocationUpsertWrapperDto)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        editAllocationForm.reset();
        this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(x => {
          this.alertService.pushAlert(new Alert(`The allocations for ${this.parcel.ParcelNumber} were successfully updated.`, AlertContext.Success));
        });
      }
        ,
        error => {
          this.isLoadingSubmit = false;
          this.cdr.detectChanges();
        }
      );
  }
}
