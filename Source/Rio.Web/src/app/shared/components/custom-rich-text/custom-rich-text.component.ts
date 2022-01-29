import { Component, OnInit, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from '../../services/alert.service';
import { Alert } from '../../models/alert';
import { AlertContext } from '../../models/enums/alert-context.enum';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { environment } from 'src/environments/environment';
import { DomSanitizer,  SafeHtml } from '@angular/platform-browser'
import { CustomRichTextDto } from '../../generated/model/custom-rich-text-dto';
import { UserDto } from '../../generated/model/user-dto';

@Component({
  selector: 'rio-custom-rich-text',
  templateUrl: './custom-rich-text.component.html',
  styleUrls: ['./custom-rich-text.component.scss']
})
export class CustomRichTextComponent implements OnInit {
  @Input() customRichTextTypeID: number;
  public customRichTextContent: SafeHtml;
  public isLoading: boolean = true;
  public isEditing: boolean = false;
  public isEmptyContent: boolean = false;
  
  public Editor = ClassicEditor;
  public editedContent: string;
  public editor;

  currentUser: UserDto;

  //For media embed https://ckeditor.com/docs/ckeditor5/latest/api/module_media-embed_mediaembed-MediaEmbedConfig.html
  //Only some embeds will work, and if we want others to work we'll likely need to write some extra functions
  public ckConfig = {mediaEmbed: {previewsInData: true}};

  constructor(private customRichTextService: CustomRichTextService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService,
    private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
    });
    //window.Editor = this.Editor;

    this.customRichTextService.getCustomRichText(this.customRichTextTypeID).subscribe(x => {
      this.customRichTextContent = this.sanitizer.bypassSecurityTrustHtml(x.CustomRichTextContent);
      this.editedContent = x.CustomRichTextContent;
      this.isEmptyContent = x.IsEmptyContent;
      this.isLoading = false;
    });
  }

  // tell CkEditor to use the class below as its upload adapter
  // see https://ckeditor.com/docs/ckeditor5/latest/framework/guides/deep-dive/upload-adapter.html#how-does-the-image-upload-work
  public ckEditorReady(editor) {
    const customRichTextService = this.customRichTextService
    this.editor = editor;

    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
      // disable the editor until the image comes back
      editor.isReadOnly = true;
      return new CkEditorUploadAdapter(loader, customRichTextService, environment.mainAppApiUrl, editor);
    };
  }

  public showEditButton(): boolean {
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public enterEdit(): void {
    this.isEditing = true;
  }

  public cancelEdit(): void {
    this.isEditing = false;
  }

  public saveEdit(): void {
    this.isEditing = false;
    this.isLoading = true;
    const updateDto = new CustomRichTextDto({ CustomRichTextContent: this.editedContent });
    this.customRichTextService.updateCustomRichText(this.customRichTextTypeID, updateDto).subscribe(x => {
      this.customRichTextContent = this.sanitizer.bypassSecurityTrustHtml(x.CustomRichTextContent);
      this.editedContent = x.CustomRichTextContent
      this.isLoading = false;
    }, error => {
      this.isLoading = false;
      this.alertService.pushAlert(new Alert("There was an error updating the rich text content", AlertContext.Danger, true));
    });
  }
  
  public isUploadingImage():boolean{
    return this.editor && this.editor.isReadOnly;
  }

}

class CkEditorUploadAdapter {
  loader;
  service: CustomRichTextService;
  apiUrl: string;
  editor;

  constructor(loader, uploadService: CustomRichTextService, apiUrl: string, editor) {
    // The file loader instance to use during the upload.
    this.loader = loader;
    this.service = uploadService;
    this.apiUrl = apiUrl;
    this.editor = editor;
  }

  // Starts the upload process.
  upload() {
    const editor = this.editor;
    const service = this.service;


    return this.loader.file.then(file => new Promise((resolve, reject) => {
      service.uploadFile(file).subscribe(x => {
        const imageUrl = `${this.apiUrl}${x.imageUrl}`;
        editor.isReadOnly = false;

        resolve({
          // todo: this should be correct instead of incorrect.
          default: imageUrl
        });
      }, error => {
        editor.isReadOnly = false;

        reject("There was an error uploading the file. Please try again.")
      });
    })
    );
  }

  // Aborts the upload process.
  abort() {
    // NP 4/23/2020 todo? I'm not sure this is actually necessary, I don't see any way for the user to cancel the upload once triggered.
  }
}
