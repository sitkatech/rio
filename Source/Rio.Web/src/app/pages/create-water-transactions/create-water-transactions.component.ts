import { Component, OnInit, OnDestroy, ChangeDetectorRef, ViewChild, ViewChildren, QueryList, ElementRef } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { AlertService } from 'src/app/shared/services/alert.service';
import { ParcelAllocationUpsertDto } from 'src/app/shared/models/parcel/parcel-allocation-upsert-dto.';
import { AlertContext } from 'src/app/shared/models/enums/alert-context.enum';
import { Alert } from 'src/app/shared/models/alert';
import { UserDto } from 'src/app/shared/models';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { ParcelAllocationHistoryDto } from 'src/app/shared/models/parcel/parcel-allocation-history-dto';
import { forkJoin } from 'rxjs';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { WaterTypeApplicationTypeEnum, WaterTypeDto } from 'src/app/shared/models/water-type-dto';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { NgbModal, NgbModalRef } from '@ng-bootstrap/ng-bootstrap';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterYearDto } from "src/app/shared/models/water-year-dto";

@Component({
  selector: 'rio-create-water-transactions',
  templateUrl: './create-water-transactions.component.html',
  styleUrls: ['./create-water-transactions.component.scss']
})
export class CreateWaterTransactions implements OnInit, OnDestroy {
  @ViewChild('parcelAllocationHistoryGrid') parcelAllocationHistoryGrid: AgGridAngular;
  @ViewChild("updateWaterAllocationModalContent") updateWaterAllocationModalContent

  @ViewChildren('fileUpload') fileUploads: QueryList<any>;
  @ViewChildren('waterAllocation') waterAllocations: QueryList<any>;

  public modalReference: NgbModalRef;

  public parcelAllocationHistoryGridColumnDefs: ColDef[];
  public defaultColDef: ColDef;
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  public richTextTypeID: number = CustomRichTextType.CreateWaterTransactions;

  public model: ParcelAllocationUpsertDto;
  public isLoadingSubmit: boolean = false;
  public waterYearToDisplay: WaterYearDto;
  public waterYears: Array<WaterYearDto>;
  public allocationHistoryEntries: Array<ParcelAllocationHistoryDto>;

  public displayErrors: any = {};
  public displayFileErrors: any = {};

  public fileName: string;
  public waterTypes: WaterTypeDto[];
  public applicationTypeToUpdate: WaterTypeDto;
  public acreageBasedWaterTypes: WaterTypeDto[];
  public spreadsheetDrivenWaterTypes: WaterTypeDto[];
  public waterTypeToUpdate: WaterTypeDto;

  constructor(private cdr: ChangeDetectorRef,
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private authenticationService: AuthenticationService,
    private alertService: AlertService,
    private waterTypeService: WaterTypeService,
    private datePipe: DatePipe,
    private modalService: NgbModal) { }

  ngOnInit(): void {
    this.model = new ParcelAllocationUpsertDto();

    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.initializeParcelAllocationHistoryGrid();

      forkJoin(this.waterYearService.getDefaultWaterYearToDisplay(),
        this.waterYearService.getWaterYears(),
        this.waterTypeService.getWaterTypes()
      ).subscribe(([defaultYear, waterYears, waterTypes]) => {
        this.waterYearToDisplay = defaultYear;
        this.waterYears = waterYears;
        this.waterTypes = waterTypes;
        this.acreageBasedWaterTypes = this.waterTypes.filter(x => x.IsAppliedProportionally === WaterTypeApplicationTypeEnum.Proportional);
        this.spreadsheetDrivenWaterTypes = this.waterTypes.filter(x => x.IsAppliedProportionally === WaterTypeApplicationTypeEnum.Spreadsheet);
        this.waterTypes.forEach(x => {
          this.displayErrors[x.WaterTypeName] = false;
          this.displayFileErrors[x.WaterTypeName] = false;
        })
        this.updateParcelAllocationHistoryGrid();
      });
    });
  }

  private initializeParcelAllocationHistoryGrid(): void {
    // TODO: Date should become Transaction Date, Water Year should become Effective Date once Parcel Allocation History is updated to use Parcel Ledger
    this.parcelAllocationHistoryGridColumnDefs = [
      this.createDateColumnDef('Date', 'Date', 'M/d/yyyy'),
      { headerName: 'Water Year', field: 'WaterYear'},
      { headerName: 'Supply Type', field: 'Allocation', width: 150 },
      { headerName: 'Value (ac-ft/ac)', field: 'Value', width: 150, cellStyle: {textAlign: "right"},
        valueGetter: function (params: any) {
          return params.data.Value ?? '-';
        } 
      },
      { headerName: 'Uploaded Filename', field: 'Filename', width: 275 },
      { headerName: 'User', field: 'User'}
    ];

    this.defaultColDef = {
      resizable: true, sortable: true, filter: true, width: 130
    };
  }

  private dateFilterComparator(filterLocalDateAtMidnight, cellValue) {
    const cellDate = Date.parse(cellValue);
    if (cellDate == filterLocalDateAtMidnight) {
      return 0;
    }
    return (cellDate < filterLocalDateAtMidnight) ? -1 : 1;
  }
  
  private createDateColumnDef(headerName: string, fieldName: string, dateFormat: string): ColDef {
    let datePipe = this.datePipe;
    
    return {
      headerName: headerName, valueGetter: function (params: any) {
        return datePipe.transform(params.data[fieldName], dateFormat);
      },
      comparator: this.dateFilterComparator, sortable: true, filter: 'agDateColumnFilter',
      filterParams: {
        filterOptions: ['inRange'],
        comparator: this.dateFilterComparator
      }
    };
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public updateParcelAllocationHistoryGrid(): void {
    this.parcelService.getParcelAllocationHistory().subscribe(parcelAllocationHistory => {
      this.allocationHistoryEntries = parcelAllocationHistory;
      this.parcelAllocationHistoryGrid ? this.parcelAllocationHistoryGrid.api.setRowData(parcelAllocationHistory) : null;
    });
  }
}
