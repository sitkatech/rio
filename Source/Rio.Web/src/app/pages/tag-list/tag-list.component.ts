import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TagService } from 'src/app/services/tag/tag.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';

@Component({
  selector: 'rio-tag-list',
  templateUrl: './tag-list.component.html',
  styleUrls: ['./tag-list.component.scss']
})
export class TagListComponent implements OnInit {
  @ViewChild('tagsGrid') tagsGrid: AgGridAngular;

  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public columnDefs: Array<ColDef>;
  public rowData: Array<TagDto>;
  public richTextTypeID = CustomRichTextType.TagList;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private tagService: TagService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.cdr.detectChanges();

      this.createTagsGridColumnDefs();
      this.tagService.getAllTags().subscribe(tags => {
        this.tagsGrid.api.setRowData(tags);
      });
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  private createTagsGridColumnDefs() {
    this.columnDefs = [
      { headerName: 'Tag Name', field: 'TagName', sortable: true, filter: true, resizable: true, width: 280 },
      { headerName: 'Tag Description', field: 'TagDescription', sortable: true, filter: true, resizable: true, width: 450 },
      this.utilityFunctionsService.createDecimalColumnDef('Tagged Parcels Count', 'TaggedParcelsCount', 180, 0)
    ]
  }

}
