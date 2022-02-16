import { ChangeDetectorRef, Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { TagService } from 'src/app/services/tag/tag.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { ParcelSimpleDto } from 'src/app/shared/generated/model/parcel-simple-dto';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';

@Component({
  selector: 'rio-tag-detail',
  templateUrl: './tag-detail.component.html',
  styleUrls: ['./tag-detail.component.scss']
})
export class TagDetailComponent implements OnInit, OnDestroy {
  @ViewChild('taggedParcelsIndexGrid') taggedParcelsIndexGrid: AgGridAngular;

  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public tag: TagDto;
  public columnDefs: Array<ColDef>;
  public taggedParcels: Array<ParcelSimpleDto>;

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private tagService: TagService,
    private parcelService: ParcelService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      this.tagService.getTagByID(id).subscribe(tag => {
        this.tag = tag;

        this.createTaggedParcelsIndexGridColumnDefs();
        this.parcelService.getByTagID(tag.TagID).subscribe(taggedParcels => {
          this.taggedParcels = taggedParcels;
          this.taggedParcelsIndexGrid.api.setRowData(taggedParcels);
        });
      });

      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  private createTaggedParcelsIndexGridColumnDefs() {
    this.columnDefs = [
      {
        headerName: 'APN', valueGetter: function (params: any) {
          return { LinkDisplay: params.data.ParcelNumber, LinkValue: params.data.ParcelID };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/parcels/" },
        filterValueGetter: params => params.data.ParcelNumber,
        sortable: true, filter: true, width: 100
      },
    ]
  }
}
