import { Component, OnInit, ChangeDetectorRef, OnDestroy } from '@angular/core';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingService } from 'src/app/services/posting.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';

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
}
