import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/models/parcel/parcel-dto';
import { ParcelAllocationAndConsumptionDto } from 'src/app/shared/models/parcel/parcel-allocation-and-consumption-dto';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { Alert } from 'src/app/shared/models/alert';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { ParcelAllocationUpsertWrapperDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-wrapper-dto.';
import { UserDto } from 'src/app/shared/models';

@Component({
  selector: 'rio-parcel-edit-allocation',
  templateUrl: './parcel-edit-allocation.component.html',
  styleUrls: ['./parcel-edit-allocation.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class ParcelEditAllocationComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public parcel: ParcelDto;
  public parcelAllocationAndConsumptions: Array<ParcelAllocationAndConsumptionDto>;
  public model: ParcelAllocationUpsertWrapperDto;
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
          this.parcelService.getParcelAllocationAndConsumption(id)
        ).subscribe(([parcel, parcelAllocationAndConsumptions]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          this.parcel = parcel;
          this.parcelAllocationAndConsumptions = parcelAllocationAndConsumptions;
          this.model = new ParcelAllocationUpsertWrapperDto();
          this.model.ParcelAllocations =
            this.parcelAllocationAndConsumptions.map(x => {
              console.log(x);
              let parcelAllocationUpsertDto = new ParcelAllocationUpsertDto();
              parcelAllocationUpsertDto.WaterYear = x.WaterYear;
              parcelAllocationUpsertDto.AcreFeetAllocated = x.AcreFeetAllocated;
              return parcelAllocationUpsertDto;
            }
            );
          this.cdr.detectChanges();
        });
      }
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public onSubmit(editAllocationForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.parcelService.updateAnnualAllocations(this.parcel.ParcelID, this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        editAllocationForm.reset();
        this.router.navigateByUrl("/parcels/" + this.parcel.ParcelID).then(x => {
          this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
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
