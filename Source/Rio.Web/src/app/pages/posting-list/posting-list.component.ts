import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingService } from 'src/app/services/posting.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';

@Component({
  selector: 'rio-posting-list',
  templateUrl: './posting-list.component.html',
  styleUrls: ['./posting-list.component.scss']
})
export class PostingListComponent implements OnInit, OnDestroy {
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  descriptionMaxLength: number = 300;
  postings: PostingDto[];
  postingToEdit = {};

  constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private postingService: PostingService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.postingService.getPostings().subscribe(result => {
        this.postings = result;
        this.cdr.detectChanges();
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getPostingsToBuy() : Array<PostingDto>
  {
    return this.postings ? this.postings.filter(x => x.PostingType.PostingTypeID === PostingTypeEnum.OfferToBuy) : [];
  }

  public getPostingsToSell() : Array<PostingDto>
  {
    return this.postings ? this.postings.filter(x => x.PostingType.PostingTypeID === PostingTypeEnum.OfferToSell) : [];
  }

  public getAcreFeetToSell() : number
  {
    return this.getPostingsToSell().reduce(function (a, b) {
      return (a + b.AvailableQuantity);
    }, 0);
  }

  public getAcreFeetToBuy() : number
  {
    return this.getPostingsToBuy().reduce(function (a, b) {
      return (a + b.AvailableQuantity);
    }, 0);
  }
}
