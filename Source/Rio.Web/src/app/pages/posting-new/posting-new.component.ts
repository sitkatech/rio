import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { Router } from '@angular/router';
import { PostingService } from 'src/app/services/posting.service';
import { PostingTypeDto } from 'src/app/shared/models/postingType/role-dto';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { PostingTypeService } from 'src/app/services/posting-type.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { AuthenticationService } from 'src/app/services/authentication.service';

@Component({
  selector: 'rio-posting-new',
  templateUrl: './posting-new.component.html',
  styleUrls: ['./posting-new.component.scss']
})
export class PostingNewComponent implements OnInit {

  public postingTypes: Array<PostingTypeDto>;
  public model: any = {};
  public isLoadingSubmit: boolean = false;

  constructor(
    private cdr: ChangeDetectorRef, 
    private router: Router, 
    private postingService: PostingService, 
    private postingTypeService: PostingTypeService, 
    private alertService: AlertService,
    private authenticationService: AuthenticationService
    ) { }

  ngOnInit(): void {
    this.postingTypeService.getPostingTypes().subscribe(result => {
      this.postingTypes = result;
      this.cdr.detectChanges();
    });
  }

  onSubmit(invitePostingForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;
    this.model.CreateUserID = this.authenticationService.currentUser.UserID;
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
}
