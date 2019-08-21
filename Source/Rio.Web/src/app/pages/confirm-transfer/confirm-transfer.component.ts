import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { TradeDto } from 'src/app/shared/models/offer/trade-dto';
import { OfferDto } from 'src/app/shared/models/offer/offer-dto';
import { PostingTypeDto } from 'src/app/shared/models/posting/posting-type-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { OfferService } from 'src/app/services/offer.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { forkJoin } from 'rxjs';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { OfferStatusEnum } from 'src/app/shared/models/enums/offer-status-enum';
import { TradeStatusEnum } from 'src/app/shared/models/enums/trade-status-enum';
import { WaterTransferService } from 'src/app/services/water-transfer.service';
import { WaterTransferDto } from 'src/app/shared/models/water-year-transaction-dto';

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

  constructor(
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private waterTransferService: WaterTransferService,
    private authenticationService: AuthenticationService
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
  }
}
