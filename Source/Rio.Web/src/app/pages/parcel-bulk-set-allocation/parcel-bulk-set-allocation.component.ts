import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { forkJoin } from 'rxjs';

@Component({
  selector: 'rio-parcel-bulk-set-allocation',
  templateUrl: './parcel-bulk-set-allocation.component.html',
  styleUrls: ['./parcel-bulk-set-allocation.component.scss']
})
export class ParcelBulkSetAllocationComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: number;
  public waterYears: Array<number>;
  public parcelAllocationTypes = [{ParcelAllocationTypeID: 1, Name: "Project Water"}, {ParcelAllocationTypeID: 2, Name: "Reconciliation"}, {ParcelAllocationTypeID: 3, Name: "Native Yield"}];

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router, private parcelService: ParcelService, private authenticationService: AuthenticationService, private alertService: AlertService) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      
      this.parcelService.getDefaultWaterYearToDisplay().subscribe(defaultYear=>{
        this.waterYearToDisplay = defaultYear;
        this.parcelService.getWaterYears().subscribe(waterYears =>{
          this.waterYears = waterYears;
        }) 
      })     

    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  onSubmit(bulkSetAllocationForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.parcelService.bulkSetAnnualAllocations(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/manager-dashboard").then(x => {
          this.alertService.pushAlert(new Alert("Successfully set Allocations for " + response + " parcels.", AlertContext.Success));
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
