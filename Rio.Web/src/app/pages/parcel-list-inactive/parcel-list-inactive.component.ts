import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { DatePipe, DecimalPipe } from '@angular/common';
import { GridOptions } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { AgGridAngular } from 'ag-grid-angular';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { CustomRichTextTypeEnum } from 'src/app/shared/generated/enum/custom-rich-text-type-enum';
import { WaterYearService } from 'src/app/services/water-year.service';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';

@Component({
  selector: 'rio-parcel-list',
  templateUrl: './parcel-list-inactive.component.html',
  styleUrls: ['./parcel-list-inactive.component.scss']
})
export class ParcelListInactiveComponent implements OnInit, OnDestroy {
  @ViewChild('parcelsGrid') parcelsGrid: AgGridAngular;

  public richTextTypeID: number = CustomRichTextTypeEnum.InactiveParcelList;

  
  private currentUser: UserDto;

  public waterYears: Array<WaterYearDto>;
  public waterYearToDisplay: WaterYearDto;
  public gridOptions: GridOptions;
  public rowData = [];
  public mapHeight: string = "500px"
  public columnDefs: any;

  public gridApi: any;
  public highlightedParcel: any;
  public selectedParcelIDs: Array<number> = [];

  private _highlightedParcelID: number;
  public loadingParcels: boolean = true;
  public selectedParcelsLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Account Parcels";
  public set highlightedParcelID(value: number) {
    if (value != this._highlightedParcelID) {
      this._highlightedParcelID = value;
      this.highlightedParcel = this.rowData.filter(x => x.ParcelID == value)[0];
      this.selectHighlightedParcelIDRowNode();
    }
  }
  public get highlightedParcelID() {
    return this._highlightedParcelID;
  }

  constructor(private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private utilityFunctionsService: UtilityFunctionsService,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private waterTypeService: WaterTypeService,
    private decimalPipe: DecimalPipe,
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {

      let _datePipe = this.datePipe;
      this.columnDefs = [
        {
          headerName: 'APN', valueGetter: function (params: any) {
            return { LinkDisplay: params.data.ParcelNumber, LinkValue: params.data.ParcelID };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/parcels/" },
          filterValueGetter: function (params: any) {
            return params.data.ParcelNumber;
          },
          comparator: function (id1: any, id2: any) {
            if (id1.LinkDisplay < id2.LinkDisplay) {
              return -1;
            }
            if (id1.LinkDisplay > id2.LinkDisplay) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true
        },
        { headerName: 'Inactivate Date', field: 'InactivateDate', valueFormatter: function (params) {
          return _datePipe.transform(params.value, "M/d/yyyy, h:mm a")
        },
        filterValueGetter: function (params: any) {
          return _datePipe.transform(params.data.InactivateDate, "M/d/yyyy");
        },
        filterParams: {
          // provide comparator function
          comparator: function (filterLocalDate, cellValue) {
            var dateAsString = cellValue;
            if (dateAsString == null) return -1;
            var cellDate = Date.parse(dateAsString);
            const filterLocalDateAtMidnight = filterLocalDate.getTime();
            if (filterLocalDateAtMidnight == cellDate) {
              return 0;
            }
            if (cellDate < filterLocalDateAtMidnight) {
              return -1;
            }
            if (cellDate > filterLocalDateAtMidnight) {
              return 1;
            }
          }
        },
        comparator: function (id1: any, id2: any) {
          let time1 = id1 ? Date.parse(id1) : 0;
          let time2 = id2 ? Date.parse(id2) : 0;

          if (time1 < time2) {
            return -1;
          }
          if (time1 > time2) {
            return 1;
          }
          return 0;
        },
        sortable: true, filter: 'agDateColumnFilter'}
      ];

      this.gridOptions = <GridOptions>{};
      this.currentUser = currentUser;
      this.parcelsGrid?.api.showLoadingOverlay();

      this.parcelService.getInactiveParcels().subscribe((parcels) => {
        this.rowData = parcels;
        this.selectedParcelIDs = this.rowData.map(x => x.ParcelID);
        this.parcelsGrid.api.hideOverlay();
        setTimeout(() => {
          this.parcelsGrid.columnApi.autoSizeAllColumns();
        }, 50);
        this.loadingParcels = false;
        this.cdr.detectChanges();
      });

    });
  }

  ngOnDestroy() {
    
    
    this.cdr.detach();
  }

  public exportToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelsGrid, 'inactive-parcels.csv', null);
  }

  public onGridReady(params) {
    this.gridApi = params.api;
  }

  public onSelectionChanged(event) {
    let selection = this.gridApi.getSelectedRows()[0];
    if (selection && selection.ParcelID) {
      this.highlightedParcelID = selection.ParcelID;
    }
  }

  public selectHighlightedParcelIDRowNode() {
    this.gridApi.forEachNodeAfterFilterAndSort((rowNode, index) => {
      if (rowNode.data.ParcelID == this.highlightedParcelID) {
        rowNode.setSelected(true);
        this.gridApi.ensureIndexVisible(index);
      }
    });
  }
}
