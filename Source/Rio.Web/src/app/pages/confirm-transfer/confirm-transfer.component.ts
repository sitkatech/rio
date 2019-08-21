import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { WaterTransferService } from 'src/app/services/water-transfer.service';
import { WaterTransferDto } from 'src/app/shared/models/water-transfer-dto';
import { WaterTransferConfirmDto } from 'src/app/shared/models/water-transfer-confirm-dto';
import { WaterTransferTypeEnum } from 'src/app/shared/models/enums/water-transfer-type-enum';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';

@Component({
  selector: 'rio-confirm-transfer',
  templateUrl: './confirm-transfer.component.html',
  styleUrls: ['./confirm-transfer.component.scss']
})
export class ConfirmTransferComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public waterTransfer: WaterTransferDto;
  public isConfirmingTransfer: boolean = false;
  public confirmAction: string;
  public isLoadingSubmit: boolean = false;

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private waterTransferService: WaterTransferService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      const waterTransferID = parseInt(this.route.snapshot.paramMap.get("waterTransferID"));
      if (waterTransferID) {
        this.getData(waterTransferID);
      }
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  private getData(waterTransferID: number): void {
    forkJoin(this.waterTransferService.getWaterTransferFromWaterTransferID(waterTransferID)).subscribe(([waterTransfer]) => {
      this.waterTransfer = waterTransfer instanceof Array
        ? null
        : waterTransfer as WaterTransferDto;
      this.isConfirmingTransfer = false;
      this.confirmAction = this.waterTransfer.ReceivingUser.UserID === this.currentUser.UserID ? "to" : "from";
    });
  }

  public canConfirm(): boolean {
    return (this.waterTransfer.ReceivingUser.UserID === this.currentUser.UserID && this.waterTransfer.ConfirmedByReceivingUser === false) ||
    (this.waterTransfer.TransferringUser.UserID === this.currentUser.UserID && this.waterTransfer.ConfirmedByTransferringUser === false);
  }

  public confirmTransfer(): void {
    this.isConfirmingTransfer = true;
  }

  public cancelConfirmation(): void {
    this.isConfirmingTransfer = false;
  }

  public submitConfirmation(): void {
    this.isLoadingSubmit = true;
    let model = new WaterTransferConfirmDto();
    model.ConfirmingUserID = this.currentUser.UserID;
    model.WaterTransferType = this.waterTransfer.ReceivingUser.UserID === this.currentUser.UserID ? WaterTransferTypeEnum.Receiving : WaterTransferTypeEnum.Transferring;
    this.waterTransferService.confirmTransfer(this.waterTransfer.WaterTransferID, model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        this.router.navigateByUrl("/landowner-dashboard")
          .then(() => {
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
