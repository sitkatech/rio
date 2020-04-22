import { Component, OnInit, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';

@Component({
  selector: 'rio-custom-rich-text',
  templateUrl: './custom-rich-text.component.html',
  styleUrls: ['./custom-rich-text.component.scss']
})
export class CustomRichTextComponent implements OnInit {
  @Input() customRichTextTypeID: number;
  public customRichTextContent: string;
  public isLoading: boolean = true;
  public isEditing: boolean = false;
  public isEmptyContent: boolean = false;
  public watchUserChangeSubscription: any;
  public Editor = ClassicEditor;

  currentUser: UserDto;

  constructor(private customRichTextService: CustomRichTextService, private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef ) { }
  
  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
        this.currentUser = currentUser;
    });
    

    this.customRichTextService.getCustomRichText(this.customRichTextTypeID).subscribe(x=>{
      this.customRichTextContent = x.CustomRichTextContent;
      this.isEmptyContent = x.IsEmptyContent;
      this.isLoading = false;
    });
  }

  public showEditButton() : boolean{
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public enterEdit(): void{
    this.isEditing = true;
  }

  public cancelEdit(): void{
    this.isEditing = false;
  }
}
