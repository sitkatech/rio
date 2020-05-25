import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { PostingService } from 'src/app/services/posting.service';
import { PostingTypeDto } from 'src/app/shared/models/posting/posting-type-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { PostingTypeService } from 'src/app/services/posting-type.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { isNullOrUndefined } from 'util';
import { UserDto } from 'src/app/shared/models';
import { PostingUpsertDto } from 'src/app/shared/models/posting/posting-upsert-dto';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';

@Component({
  selector: 'rio-posting-new',
  templateUrl: './posting-new.component.html',
  styleUrls: ['./posting-new.component.scss']
})
export class PostingNewComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  public postingTypes: Array<PostingTypeDto>;
  public model: PostingUpsertDto = new PostingUpsertDto();
  public isLoadingSubmit: boolean = false;
  currentAccount: AccountSimpleDto;

  constructor(
    private cdr: ChangeDetectorRef,
    private router: Router,
    private postingService: PostingService,
    private postingTypeService: PostingTypeService,
    private alertService: AlertService,
    private authenticationService: AuthenticationService
  ) { }

  ngOnInit(): void {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.authenticationService.getActiveAccount().subscribe(account=>{
        this.currentAccount = account;
      })
      this.postingTypeService.getPostingTypes().subscribe(result => {
        this.postingTypes = result;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  onSubmit(invitePostingForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.model.CreateAccountID = this.currentAccount.AccountID;
    this.postingService.newPosting(this.model)
      .subscribe(response => {
        this.isLoadingSubmit = false;
        invitePostingForm.reset();
        this.router.navigateByUrl("/trades").then(x => {
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

  public getTotalPrice(): number {
    if (isNullOrUndefined(this.model.Price) || isNullOrUndefined(this.model.Quantity)) {
      return null;
    }
    return this.model.Price * this.model.Quantity;
  }

  public isOfferFormValid(): boolean {
      return this.model.Price > 0 && this.model.Quantity > 0 && this.model.PostingTypeID > 0;
  }
}