import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { forkJoin } from 'rxjs';
import { NgbModalRef, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AlertService } from 'src/app/shared/services/alert.service';
import { Alert } from 'src/app/shared/models/alert';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { TagService } from 'src/app/services/tag/tag.service';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { WaterYearService } from 'src/app/services/water-year.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { ParcelWaterSupplyAndUsageDto } from 'src/app/shared/generated/model/parcel-water-supply-and-usage-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { TagBulkSetUpsertDto } from 'src/app/shared/generated/model/tag-bulk-set-upsert-dto';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';

@Component({
  selector: 'rio-tag-bulk-parcels',
  templateUrl: './tag-bulk-parcels.component.html',
  styleUrls: ['./tag-bulk-parcels.component.scss']
})
export class TagBulkParcelsComponent implements OnInit {
  @ViewChild('bulkTagParcelsGrid') bulkTagParcelsGrid: AgGridAngular;
  @ViewChild('applyTagModal') applyTagModal: NgbModal;

  private watchAccountChangeSubscription: any;
  private currentUser: UserDto;

  public customRichTextTypeID = CustomRichTextTypeEnum.BulkTagParcels;
  public parcelWaterSupplyAndUsages: Array<ParcelWaterSupplyAndUsageDto>;
  public waterYearToDisplay: WaterYearDto;
  public waterYears: Array<WaterYearDto>;
  public columnDefs: Array<ColDef>;
  private modalReference: NgbModalRef;
  public tagModel: TagDto;
  public parcelsToTag: Array<ParcelWaterSupplyAndUsageDto>;
  public isLoadingSubmit = false;
  public noParcelsSelected = true;

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private waterYearService: WaterYearService,
    private tagService: TagService,
    private parcelService: ParcelService,
    private modalService: NgbModal,
    private alertService: AlertService
  ) { }

  ngOnInit(): void {
    this.watchAccountChangeSubscription = this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;

      this.createBulkTagParcelsGridColumnDefs();
      forkJoin({
        defaultWaterYear: this.waterYearService.getDefaultWaterYearToDisplay(),
        waterYears: this.waterYearService.getWaterYears()
      }).subscribe(({defaultWaterYear, waterYears}) => {
        this.waterYearToDisplay = defaultWaterYear;
        this.waterYears = waterYears;

        this.updateGridData();
      });

      this.cdr.detectChanges();
    });
  }

  ngOnDestroy() {
    this.watchAccountChangeSubscription.unsubscribe();
    this.cdr.detach();
  }

  private createBulkTagParcelsGridColumnDefs() {
    this.columnDefs = [
      { filter: false, sortable: false, checkboxSelection: true, headerCheckboxSelection: true, headerCheckboxSelectionFilteredOnly: true, width: 40 },
      {
        headerName: 'APN', valueGetter: function (params: any) {
          return { LinkDisplay: params.data.ParcelNumber, LinkValue: params.data.ParcelID };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/parcels/" },
        filterValueGetter: params => params.data.ParcelNumber,
        comparator: this.utilityFunctionsService.linkRendererComparator,
        sortable: true, filter: true, width: 100
      },
      this.utilityFunctionsService.createDecimalColumnDef('Area (acres)', 'ParcelAreaInAcres', 120, 0),
      {
        headerName: 'Account', valueGetter: function (params: any) {
          return { LinkValue: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountID, LinkDisplay: params.data.LandOwner === null ? "" : params.data.LandOwner.AccountDisplayName };
        }, cellRendererFramework: LinkRendererComponent,
        cellRendererParams: { inRouterLink: "/accounts/" },
        filterValueGetter: params =>(params.data.LandOwner) ? params.data.LandOwner.AccountDisplayName : null,
        comparator: this.utilityFunctionsService.linkRendererComparator,
        sortable: true, filter: true, width: 170
      },
      { headerName: 'Tags', field: 'TagsAsCommaSeparatedString', filter: true, sortable: true, resizable: true },
      this.utilityFunctionsService.createDecimalColumnDef('Total Supply', 'TotalSupply', 150),
      this.utilityFunctionsService.createDecimalColumnDef('Precipitation', 'Precipitation', 130),
      this.utilityFunctionsService.createDecimalColumnDef('Total Usage', 'UsageToDate', 130)
    ];
  }

  public updateGridData() {
    if (!this.waterYearToDisplay) {
      return;
    }

    this.parcelService.getParcelWaterSupplyAndUsagesByYear(this.waterYearToDisplay.Year).subscribe(parcelWaterSupplyAndUsages => {
      this.parcelWaterSupplyAndUsages = parcelWaterSupplyAndUsages;
      this.bulkTagParcelsGrid.api.setRowData(parcelWaterSupplyAndUsages);
    });
  }

  public onSelectionChanged() {
    this.noParcelsSelected = this.bulkTagParcelsGrid.api.getSelectedRows().length == 0;
  }

  private launchModal(modalContent: any, modalTitle: string): void {
    this.modalReference = this.modalService.open(
      modalContent, 
      { ariaLabelledBy: modalTitle, beforeDismiss: () => this.checkIfSubmitting(), backdrop: 'static', keyboard: false 
    });
  }

  private checkIfSubmitting(): boolean {
    return this.isLoadingSubmit;
  }

  public tagSelectedParcels() {
    this.tagModel = new TagDto();
    this.parcelsToTag = this.bulkTagParcelsGrid.api.getSelectedRows();
    this.launchModal(this.applyTagModal, 'applyTagModalTitle');
  }

  public onSubmit() {
    this.isLoadingSubmit = true;
    this.alertService.clearAlerts();

    var tagBulkSetUpsertDto = new TagBulkSetUpsertDto();
    tagBulkSetUpsertDto.TagDto = this.tagModel;
    tagBulkSetUpsertDto.parcelIDs = this.parcelsToTag.map(x => x.ParcelID);
    
    this.tagService.bulkTagParcel(tagBulkSetUpsertDto).subscribe(result => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
      this.alertService.pushAlert(new Alert(`${result} parcels successfully tagged`, AlertContext.Success, true));
      this.updateGridData();
    }, error => {
      this.isLoadingSubmit = false;
      this.modalReference.close();
    });
  }
}
