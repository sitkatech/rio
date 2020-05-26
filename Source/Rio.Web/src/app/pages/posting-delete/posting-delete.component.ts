import { Component, OnInit, OnDestroy, ChangeDetectorRef } from '@angular/core';
import { UserDto } from 'src/app/shared/models';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { OfferDto } from 'src/app/shared/models/offer/offer-dto';
import { ActivatedRoute, Router } from '@angular/router';
import { PostingService } from 'src/app/services/posting.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { forkJoin } from 'rxjs';
import { PostingStatusEnum } from 'src/app/shared/models/enums/posting-status-enum';
import { PostingUpdateStatusDto } from 'src/app/shared/models/posting/posting-update-status-dto';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { TradeDto } from 'src/app/shared/models/offer/trade-dto';

@Component({
  selector: 'rio-posting-delete',
  templateUrl: './posting-delete.component.html',
  styleUrls: ['./posting-delete.component.scss']
})
export class PostingDeleteComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public posting: PostingDto;
  public trades: Array<TradeDto>;
  public isLoadingSubmit: boolean = false;

  public isDeletingPosting: boolean = false;
  public  successfullyDeleted: boolean = false;

  constructor(
      private cdr: ChangeDetectorRef,
      private route: ActivatedRoute,
      private router: Router,
      private postingService: PostingService,
      private authenticationService: AuthenticationService,
      private alertService: AlertService
  ) {
  }

  ngOnInit() {
      this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
          this.currentUser = currentUser;
          const postingID = parseInt(this.route.snapshot.paramMap.get("postingID"));
          if (postingID) {
              forkJoin(
                  this.postingService.getPostingFromPostingID(postingID),
                  this.postingService.getTradesFromPostingID(postingID)
              ).subscribe(([posting, offers]) => {
                  this.posting = posting instanceof Array
                      ? null
                      : posting as PostingDto;

                  this.trades = offers;
              });
          }
      });
  }

  ngOnDestroy() {
      this.watchUserChangeSubscription.unsubscribe();
      this.authenticationService.dispose();
      this.cdr.detach();
  }

  public canDeleteCurrentPosting(): boolean {
      return this.isPostingOpen() && this.trades.length == 0;
  }

  private isPostingOpen() {
      return this.posting.PostingStatus.PostingStatusID === PostingStatusEnum.Open;
  }

  public getTotalPrice(posting: any): number {
      return posting.Price * posting.Quantity;
  }

  public deletePosting(): void {
      this.isLoadingSubmit = true;
      this.postingService.deletePosting(this.posting.PostingID)
          .subscribe(response => {
              this.isLoadingSubmit = false;
              this.successfullyDeleted = true;
              this.alertService.pushAlert(new Alert("Your request was successfully submitted.", AlertContext.Success));
          }
              ,
              error => {
                  this.isLoadingSubmit = false;
                  this.cdr.detectChanges();
              }
          );
  }
}