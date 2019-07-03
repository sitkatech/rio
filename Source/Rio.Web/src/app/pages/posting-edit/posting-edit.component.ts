import { Component, OnInit, ChangeDetectionStrategy, ChangeDetectorRef } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Router, ActivatedRoute } from '@angular/router';
import { forkJoin } from 'rxjs';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { PostingUpdateDto } from 'src/app/shared/models/posting/posting-update-dto';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingTypeDto } from 'src/app/shared/models/postingType/postingType-dto';
import { PostingService } from 'src/app/services/posting.service';
import { PostingTypeService } from 'src/app/services/posting-type.service';


@Component({
  selector: 'rio-posting-edit',
  templateUrl: './posting-edit.component.html',
  styleUrls: ['./posting-edit.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PostingEditComponent implements OnInit {

  public postingID: number;
  public posting: PostingDto;
  public model: PostingUpdateDto;
  public postingTypes: Array<PostingTypeDto>;
  public isLoadingSubmit: boolean = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private authenticationService: AuthenticationService,
    private postingService: PostingService,
    private postingTypeService: PostingTypeService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService
  ) {
  }

  ngOnInit() {
    this.postingID = parseInt(this.route.snapshot.paramMap.get("id"));

    forkJoin(
      this.postingService.getPostingFromPostingID(this.postingID),
      this.postingTypeService.getPostingTypes()
    ).subscribe(([posting, postingTypes]) => {
      this.posting = posting instanceof Array
        ? null
        : posting as PostingDto;

      this.postingTypes = postingTypes;
      this.model = new PostingUpdateDto();
      this.model.PostingTypeID = posting.PostingType.PostingTypeID;
      this.model.PostingDescription = posting.PostingDescription;
      this.model.Quantity = posting.Quantity;
      this.model.Price = posting.Price;      
      this.cdr.detectChanges();
    });
  }

  onSubmit(editPostingForm: HTMLFormElement): void {
    this.isLoadingSubmit = true;

    this.postingService.updatePosting(this.postingID, this.model)
        .subscribe(response => {
            this.isLoadingSubmit = false;
            this.router.navigateByUrl("/postings/" + this.postingID).then(x => {
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
