import { Component, OnInit, Input, OnChanges, SimpleChanges, ChangeDetectorRef } from '@angular/core';
import { CustomRichTextService } from '../../services/custom-rich-text.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from '../../models';
import { CustomRichTextDto } from '../../models/custom-rich-text-dto';
import { AlertService } from '../../services/alert.service';
import { Alert } from '../../models/alert';
import { AlertContext } from '../../models/enums/alert-context.enum';
import * as ClassicEditor from '@ckeditor/ckeditor5-build-classic';
import { environment } from 'src/environments/environment';

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
  public editor;

  currentUser: UserDto;

  public ckConfig = {"removePlugins": ["MediaEmbed"]}

  constructor(private customRichTextService: CustomRichTextService,
    private authenticationService: AuthenticationService,
    private cdr: ChangeDetectorRef,
    private alertService: AlertService) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
    });
    //window.Editor = this.Editor;

    this.customRichTextService.getCustomRichText(this.customRichTextTypeID).subscribe(x => {
      this.customRichTextContent = x.CustomRichTextContent;
      this.isEmptyContent = x.IsEmptyContent;
      this.isLoading = false;
    });
  }

  // tell CkEditor to use the class below as its upload adapter
  // see https://ckeditor.com/docs/ckeditor5/latest/framework/guides/deep-dive/upload-adapter.html#how-does-the-image-upload-work
  public ckEditorReady(editor) {
    const customRichTextService = this.customRichTextService
    this.editor = editor;

    console.log(this.Editor.builtinPlugins.map( plugin => plugin.pluginName ));

    editor.plugins.get('FileRepository').createUploadAdapter = (loader) => {
      // disable the editor until the image comes back
      editor.isReadOnly = true;
      return new CkEditorUploadAdapter(loader, customRichTextService, environment.apiHostName, editor);
    };
  }

  public showEditButton(): boolean {
    return this.authenticationService.isUserAnAdministrator(this.currentUser);
  }

  public enterEdit(): void {
    this.editedContent = this.customRichTextContent;
    this.isEditing = true;
  }

  public cancelEdit(): void {
    this.isEditing = false;
  }

  public saveEdit(): void {
    this.isEditing = false;
    this.isLoading = true;
    const updateDto = new CustomRichTextDto({ CustomRichTextContent: this.editedContent });
    console.log(updateDto);
    this.customRichTextService.updateCustomRichText(this.customRichTextTypeID, updateDto).subscribe(x => {
      this.customRichTextContent = x.CustomRichTextContent;
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
        const imageUrl = `https://${this.apiUrl}${x.imageUrl}`;
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
