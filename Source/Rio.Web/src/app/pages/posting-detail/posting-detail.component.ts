import { Component, OnInit } from '@angular/core';
import { PostingService } from 'src/app/services/posting.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { isNullOrUndefined } from 'util';

@Component({
    selector: 'template-posting-detail',
    templateUrl: './posting-detail.component.html',
    styleUrls: ['./posting-detail.component.scss']
})
export class PostingDetailComponent implements OnInit {

    public posting: PostingDto;

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private postingService: PostingService,
        private authenticationService: AuthenticationService
    ) {
    }

    ngOnInit() {
        const id = parseInt(this.route.snapshot.paramMap.get("id"));
        if (id) {
            forkJoin(
                this.postingService.getPostingFromPostingID(id)
            ).subscribe(([posting]) => {
                this.posting = posting instanceof Array
                    ? null
                    : posting as PostingDto;
            });
        }
    }

    public canEditCurrentPosting(): boolean {
        return this.authenticationService.isAdministrator() || this.posting.CreateUser.UserID == this.authenticationService.currentUser.UserID;
    }

    public getTotalPrice() : number
    {
      return this.posting.Price * this.posting.Quantity;
    }  
}
