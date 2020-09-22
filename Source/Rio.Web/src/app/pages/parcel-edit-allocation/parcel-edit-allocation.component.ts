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
import { ParcelAllocationTypeService } from 'src/app/services/parcel-allocation-type.service';
import { ParcelAllocationTypeDto } from 'src/app/shared/models/parcel-allocation-type-dto';

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
  parcelAllocationTypes: ParcelAllocationTypeDto[];

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private alertService: AlertService,
    private authenticationService: AuthenticationService,
    private parcelAllocationTypeService: ParcelAllocationTypeService,
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
          this.parcelService.getWaterYears(),
          this.parcelAllocationTypeService.getParcelAllocationTypes()
        ).subscribe(
          ([parcel, parcelAllocations, waterYears, parcelAllocationTypes]) => {
            this.parcel = parcel instanceof Array
              ? null
              : parcel as ParcelDto;
            this.parcel = parcel;
            this.parcelAllocations = parcelAllocations;
            this.waterYears = waterYears;

            console.log(parcelAllocations);

            this.parcelAllocationTypes = parcelAllocationTypes;

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
  
  public getAllocationForYearByType(parcelAllocationType: ParcelAllocationTypeDto, year: number): number {
    var parcelAllocation = this.parcelAllocations.find(x => x.WaterYear === year && x.ParcelAllocationTypeID === parcelAllocationType.ParcelAllocationTypeID);
    return parcelAllocation ? parcelAllocation.AcreFeetAllocated : null;
  }

  public updateAllocationModel(parcelAllocationType: ParcelAllocationTypeDto, year: number, $event: Event): void{
    const newValue = Number((<HTMLInputElement>$event.target).value);
debugger;
    let updatedAllocation = this.parcelAllocations.find(x=>x.ParcelAllocationTypeID === parcelAllocationType.ParcelAllocationTypeID && x.WaterYear === year );
    if (updatedAllocation !== null && updatedAllocation !== undefined) {
      updatedAllocation.AcreFeetAllocated = newValue;
    } else{
      const newAllocation = new ParcelAllocationDto({ParcelAllocationTypeID : parcelAllocationType.ParcelAllocationTypeID, WaterYear: year, ParcelID: this.parcel.ParcelID, AcreFeetAllocated: newValue});
      this.parcelAllocations.push(newAllocation);
    }

    this.cdr.detectChanges;
  }

  public onSubmit(editAllocationForm: HTMLFormElement): void {
         
    this.parcelService.mergeParcelAllocations(this.parcel.ParcelID, this.parcelAllocations)
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
