import { Component, OnInit, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { CustomRichTextDto } from '../../models/custom-rich-text-dto';
import { AlertService } from '../../services/alert.service';
import { Alert } from '../../models/alert';
import { AlertContext } from '../../models/enums/alert-context.enum';

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
  public editedContent: string;

  currentUser: UserDto;

  constructor(private customRichTextService: CustomRichTextService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService ) { }
  
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
    this.editedContent = this.customRichTextContent;
    this.isEditing = true;
  }

  public cancelEdit(): void{
    this.isEditing = false;
  }

  public saveEdit(): void{
    this.isEditing = false;
    this.isLoading = true;
    const updateDto = new CustomRichTextDto({CustomRichTextContent: this.editedContent});
    console.log(updateDto);
    this.customRichTextService.updateCustomRichText(this.customRichTextTypeID, updateDto).subscribe(x=>{
      this.customRichTextContent = x.CustomRichTextContent;
      this.isLoading = false;
    }, error =>{
      this.isLoading = false;
      this.alertService.pushAlert(new Alert("There was an error updating the rich text content", AlertContext.Danger, true));
    });
  }
}
