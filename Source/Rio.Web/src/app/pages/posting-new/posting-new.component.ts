import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { Router } from '@angular/router';
import { PostingService } from 'src/app/services/posting.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { PostingTypeService } from 'src/app/services/posting-type.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { PostingTypeDto } from 'src/app/shared/generated/model/posting-type-dto';
import { PostingUpsertDto } from 'src/app/shared/generated/model/posting-upsert-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';

@Component({
  selector: 'rio-posting-new',
  templateUrl: './posting-new.component.html',
  styleUrls: ['./posting-new.component.scss']
})
export class PostingNewComponent implements OnInit, OnDestroy {
  
  private currentUser: UserDto;

  public postingTypes: Array<PostingTypeDto>;
  public model: PostingUpsertDto = new PostingUpsertDto();
  public isLoadingSubmit: boolean = false;
  currentAccount: AccountSimpleDto;
  public currentUserAccounts: AccountSimpleDto[];

  constructor(
    private cdr: ChangeDetectorRef,
    private router: Router,
    private postingService: PostingService,
    private postingTypeService: PostingTypeService,
    private userService: UserService,
    private alertService: AlertService,
    private authenticationService: AuthenticationService
  ) { }

  ngOnInit(): void {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      if (this.currentUserAccounts?.length == 1) {
        this.model.CreateAccountID = this.currentUserAccounts[0].AccountID;
      };
      this.postingTypeService.getPostingTypes().subscribe(result => {
        this.postingTypes = result;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  onSubmit(invitePostingForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.model.CreateUserID = this.currentUser.UserID;
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
    if (this.model.Price === null || this.model.Price === undefined || 
      this.model.Quantity === null || this.model.Quantity === undefined) {
      return null;
    }
    return this.model.Price * this.model.Quantity;
  }

  public isOfferFormValid(): boolean {
    return this.model.Price > 0 && this.model.Quantity > 0 && this.model.PostingTypeID > 0;
  }
}