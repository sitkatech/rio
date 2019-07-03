import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingService } from 'src/app/services/posting.service';

@Component({
  selector: 'rio-posting-list',
  templateUrl: './posting-list.component.html',
  styleUrls: ['./posting-list.component.scss']
})
export class PostingListComponent implements OnInit {

  postings: PostingDto[];

  displayInvitePostingDialog: boolean = false;

  postingToEdit = {};
  displayEditPostingDialog: boolean = false;

  constructor(private cdr: ChangeDetectorRef, private postingService: PostingService) { }

  ngOnInit() {
      setTimeout(() => {
          this.getData();
      });
  }

  getData() {
      this.postingService.getPostings().subscribe(result => {
          this.postings = result;
          this.cdr.detectChanges();
      });
  }
}
