import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { TagService } from 'src/app/services/tag/tag.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { FontAwesomeIconLinkRendererComponent } from 'src/app/shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';

@Component({
  selector: 'rio-tag-list',
  templateUrl: './tag-list.component.html',
  styleUrls: ['./tag-list.component.scss']
})
export class TagListComponent implements OnInit {
  @ViewChild('tagsGrid') tagsGrid: AgGridAngular;
  @ViewChild('deleteTagModal') deleteTagModal
  @ViewChild('createTagModal') createTagModal


  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public columnDefs: Array<ColDef>;
  public tags: Array<TagDto>;
  public richTextTypeID = CustomRichTextType.TagList;
  private modalReference: NgbModalRef;
  private deleteColumnID = 0;
  private tagToDelete: TagDto;
  public tagModel: TagDto;
  public isLoadingDelete = false;
  public isLoadingSubmit = false;
  private isDisplayingDeleteColumn = false;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private tagService: TagService,
    private modalService: NgbModal,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      
      this.createTagsGridColumnDefs();
      this.updateGridData();
      
      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  private createTagsGridColumnDefs() {
    this.columnDefs = [
      {
        headerName: 'Tag Name', 
        valueGetter: function (params: any) {
          return { LinkValue: params.data.TagID, LinkDisplay: params.data.TagName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/tags/" },
        filterValueGetter: params => params.data.TagName
      },
      { headerName: 'Tag Description', field: 'TagDescription', sortable: true, filter: true, resizable: true, width: 550 },
      this.utilityFunctionsService.createDecimalColumnDef('Tagged Parcels Count', 'TaggedParcelsCount', 180, 0)
    ];

    if (this.isAdmin()) {
      const deleteTagColDef: ColDef = {
        cellRendererFramework: FontAwesomeIconLinkRendererComponent,
        cellRendererParams: { isSpan: true, fontawesomeIconName: 'trash', cssClasses: 'text-primary'},
        width: 40, resizable: true
      };
      this.columnDefs.splice(0, 0, deleteTagColDef);
      this.isDisplayingDeleteColumn = true;
    }
  }

  public isAdmin(): boolean {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  private updateGridData() {
    this.tagService.getAllTags().subscribe(tags => {
      
      this.tags = tags;
      this.tagsGrid.api.setRowData(tags);
    });
  }

  public exportToCsv() {
    var columnIDs = this.tagsGrid.columnApi.getAllGridColumns().map(x => x.getId());
    if (this.isDisplayingDeleteColumn) {
      columnIDs = columnIDs.slice(1);
    }

    this.utilityFunctionsService.exportGridToCsv(this.tagsGrid, 'tags.csv', columnIDs);
  }

  public onCellClicked(event: any): void {
    if (event.column.colId == this.deleteColumnID) {
      this.tagToDelete = this.tags.find(x => x.TagID == event.data.TagID);
      this.launchModal(this.deleteTagModal, 'deleteProjectModalTitle');
    }
  }

  private launchModal(modalContent: any, modalTitle: string): void {
    this.modalReference = this.modalService.open(
      modalContent, 
      { ariaLabelledBy: modalTitle, beforeDismiss: () => this.checkIfSubmitting(), backdrop: 'static', keyboard: false 
    });
  }

  public checkIfSubmitting(): boolean {
    return this.isLoadingSubmit;
  }

  public deleteTag() { 
    this.isLoadingSubmit = true;

    this.tagService.deleteTag(this.tagToDelete.TagID).subscribe(() => {
      this.isLoadingSubmit = false;
      this.modalReference.close();

      this.alertService.pushAlert(new Alert(`${this.tagToDelete.TagName} tag was successfully deleted.`, AlertContext.Success, true));
      this.updateGridData();
    }, error => {
      this.isLoadingSubmit = false;
      window.scroll(0,0);
      this.cdr.detectChanges();
    });
  }

  public createTag() {
    this.tagModel = new TagDto();
    this.launchModal(this.createTagModal, 'createTagModalTitle');
  }

  public onSubmit() {
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();
    
    this.tagService.createTag(this.tagModel).subscribe(() => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
      this.alertService.pushAlert(new Alert(`${this.tagModel.TagName} tag was successfully created.`, AlertContext.Success, true));
      this.updateGridData();
    }, error => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
    });
  }
}
