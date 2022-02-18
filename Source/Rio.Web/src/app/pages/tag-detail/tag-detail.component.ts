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
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'rio-tag-detail',
  templateUrl: './tag-detail.component.html',
  styleUrls: ['./tag-detail.component.scss']
})
export class TagDetailComponent implements OnInit, OnDestroy {
  @ViewChild('taggedParcelsIndexGrid') taggedParcelsIndexGrid: AgGridAngular;
  @ViewChild('editTagBasicsModal') editTagBasicsModal

  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public tag: TagDto;
  public columnDefs: Array<ColDef>;
  public taggedParcels: Array<ParcelDto>;
  private modalReference: NgbModalRef;
  public tagModel: TagDto;
  public isLoadingSubmit = false;

  constructor(
    private route: ActivatedRoute,
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private tagService: TagService,
    private parcelService: ParcelService,
    private utilityFunctionsService: UtilityFunctionsService,
    private modalService: NgbModal,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      this.tagService.getTagByID(id).subscribe(tag => {
        this.tag = tag;

        this.createTaggedParcelsIndexGridColumnDefs();
        this.parcelService.getParcelsByTagID(tag.TagID).subscribe(taggedParcels => {
          this.taggedParcels = taggedParcels;

          this.taggedParcelsIndexGrid.api.setRowData(taggedParcels);
          this.taggedParcelsIndexGrid.api.sizeColumnsToFit();
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
        comparator: function (id1: any, id2: any) {
          if (id1.LinkDisplay == id2.LinkDisplay) {
            return 0;
          }
          return id1.LinkDisplay > id2.LinkDisplay ? 1 : -1;
        },
        sortable: true, filter: true, width: 120
      },
      this.utilityFunctionsService.createDecimalColumnDef('Area (acres)', 'ParcelAreaInAcres', 120, 1),
      {
        headerName: 'Account', valueGetter: function (params: any) {
          return { LinkValue: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: params => params.data.LandOwner ? params.data.LandOwner.AccountDisplayName : null,
        comparator: function (id1: any, id2: any) {
          let link1 = id1.LinkDisplay;
          let link2 = id2.LinkDisplay;
          if (link1 == link2) {
            return 0;
          }
          return link1 < link2 ? 1 : -1;
        },
        sortable: true, filter: true, width: 310
      },
    ]
  }

  public isAdmin(): boolean {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.taggedParcelsIndexGrid, `${this.tag.TagName}-tagged-parcels.csv`, null);
  }

  private launchModal(modalContent: any, modalTitle: string): void {
    this.modalReference = this.modalService.open(
      modalContent, 
      { ariaLabelledBy: modalTitle, beforeDismiss: () => this.checkIsLoadingSubmit(), backdrop: 'static', keyboard: false 
    });
  }

  private checkIsLoadingSubmit() {
    return this.isLoadingSubmit;
  }

  public editTagBasics() {
    this.tagModel = Object.assign({}, this.tag);
    this.launchModal(this.editTagBasicsModal, 'editTagBasicsModalTitle');
  }

  public onSubmit() {
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();
    
    this.tagService.updateTag(this.tagModel).subscribe(updatedTag => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
      this.tag = updatedTag;
      this.alertService.pushAlert(new Alert(`${this.tag.TagName} tag was successfully updated.`, AlertContext.Success, true));
    }, error => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
    });
  }
}
