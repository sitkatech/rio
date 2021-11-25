import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { forkJoin } from 'rxjs';
import { ParcelDto } from 'src/app/shared/generated/model/parcel-dto';
import { ParcelLedgerDto } from 'src/app/shared/generated/model/parcel-ledger-dto';
import { ParcelOwnershipDto } from 'src/app/shared/generated/model/parcel-ownership-dto';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { WaterTypeDto } from 'src/app/shared/generated/model/water-type-dto';
import { WaterYearDto } from 'src/app/shared/generated/model/water-year-dto';
import { WaterYearService } from 'src/app/services/water-year.service';
import { WaterTypeService } from 'src/app/services/water-type.service';
import { ColDef } from 'ag-grid-community';
import { AgGridAngular } from 'ag-grid-angular';
import { DatePipe, DecimalPipe } from '@angular/common';
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss']
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
  @ViewChild('parcelLedgerGrid') parcelLedgerGrid: AgGridAngular;
  private watchUserChangeSubscription: any;
  private currentUser: UserDto;
  private currentUserAccounts: AccountSimpleDto[];

  public waterYears: Array<WaterYearDto>;
  public parcel: ParcelDto;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public allocationParcelLedgers: Array<ParcelLedgerDto>;
  public usageParcelLedgers: Array<ParcelLedgerDto>;
  public months: number[];
  public parcelOwnershipHistory: ParcelOwnershipDto[];

  public today: Date = new Date();
  public waterTypes: WaterTypeDto[];

  public parcelLedgerGridColumnDefs: ColDef[];
  public rowData = [];
  public defaultColDef: ColDef;

  private gridColumnApi;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private parcelService: ParcelService,
    private waterYearService: WaterYearService,
    private authenticationService: AuthenticationService,
    private waterTypeService: WaterTypeService,
    private cdr: ChangeDetectorRef,
    private utilityFunctionsService: UtilityFunctionsService,
    private decimalPipe: DecimalPipe
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      this.initializeLedgerGrid();
      if (id) {
        forkJoin(
          this.parcelService.getParcelByParcelID(id),
          this.parcelService.getParcelLedgerEntriesByParcelID(id),
          this.parcelService.getParcelOwnershipHistory(id),
          this.waterYearService.getWaterYears(),
          this.waterTypeService.getWaterTypes()
        ).subscribe(([parcel, parcelLedgers, parcelOwnershipHistory, waterYears, waterTypes]) => {
          this.parcel = parcel instanceof Array
            ? null
            : parcel as ParcelDto;
          
          this.parcelLedgers = parcelLedgers;
          this.rowData = parcelLedgers;
          this.allocationParcelLedgers = parcelLedgers.filter(x => x.TransactionType.TransactionTypeID == TransactionTypeEnum.Allocation);
          this.usageParcelLedgers = parcelLedgers.filter(x => 
            x.TransactionType.TransactionTypeID == TransactionTypeEnum.MeasuredUsage ||
            x.TransactionType.TransactionTypeID == TransactionTypeEnum.MeasuredUsageCorrection || 
            x.TransactionType.TransactionTypeID == TransactionTypeEnum.ManualAdjustment);
          this.waterYears = waterYears;
          this.parcelOwnershipHistory = parcelOwnershipHistory;
          this.waterTypes = waterTypes;
        });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
      
    });
  }

  initializeLedgerGrid() {
    // NOTE: using this date-time formatter gives the local date and time,
    // so the numbers will not match the database, which is in UTC datetime stamps
    let _decimalPipe = this.decimalPipe;
    this.parcelLedgerGridColumnDefs = [
      this.createDateColumnDef('Transaction Date', 'TransactionDate', 'short'),
      this.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy'),
      { headerName: 'Transaction Type', field: 'TransactionType.TransactionTypeName' },
      {
        headerName: 'Water Type', valueGetter: function (params: any) {
          return params.data.WaterType ? params.data.WaterType.WaterTypeName : '-';
        }
      },
      { 
        headerName: 'Transaction Amount', field: 'TransactionAmount', filter: 'agNumberColumnFilter', cellStyle: {textAlign: "right"},
        valueGetter: function (params: any) { return parseFloat(_decimalPipe.transform(params.data.TransactionAmount, "1.0-1")); }, 
      },
      { headerName: 'Transaction Description', field: 'TransactionDescription', sortable: false },
      { headerName: 'Comment', field: 'UserComment', filter: false, sortable: false,
        valueGetter: function (params: any) {
          return params.data.UserComment ?? '-';
        }
      }
    ];

    this.defaultColDef = {
      resizable: true,
      filter: true,
      sortable: true
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
    let datePipe = new DatePipe('en-US');
    
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

  public exportParcelLedgerGridToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelLedgerGrid, 'parcelLedgerfor' + this.parcel.ParcelNumber + '.csv', null);
  }
  
  private onGridReady(params) {
    this.gridColumnApi = params.columnApi;

    let colIDs = this.gridColumnApi.getAllColumns().map(col => col.colId);
    this.gridColumnApi.autoSizeColumns(colIDs);
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }
  
  public getTotalAllocationForYear(year: number): string {
    var parcelLedgerForYear = this.allocationParcelLedgers.filter(x => x.WaterYear === year);
    if (parcelLedgerForYear.length > 0) {
      let result = parcelLedgerForYear.reduce(function (a, b) {
        return (a + b.TransactionAmount);
      }, 0);
      return result.toFixed(1);
    }
    else {
      return "-";
    }
  }

  public getAllocationForYearByType(waterType: WaterTypeDto, year: number): string {
    var parcelLedgers = this.allocationParcelLedgers.filter(x => x.WaterYear === year && x.WaterType.WaterTypeID === waterType.WaterTypeID);
    if (parcelLedgers.length == 0) {
      return "-";
    }
    return parcelLedgers.reduce((a, b) => {
      return a + b.TransactionAmount;
    }, 0).toFixed(1);
  }

  public getConsumptionForYear(year: number): string {
    let parcelLedgersForYear = this.usageParcelLedgers.filter(x => x.WaterYear == year);
    
    if (parcelLedgersForYear.length == 0) {
      return "-";
    }
    let monthlyUsage = parcelLedgersForYear.reduce((a, b) => {
        return a + b.TransactionAmount;
      }, 0);

    return Math.abs(monthlyUsage).toFixed(1);
  }

  public getCurrentOwner(): ParcelOwnershipDto{  
    
    var currentOwner = this.parcelOwnershipHistory.filter(x=>{
      return x.WaterYear.Year == this.today.getFullYear();      
    });

    return currentOwner.length == 1 ? currentOwner[0] : null;
  }

  public getCurrentOwnerAccountNumber(): number {
    let currentOwner = this.getCurrentOwner();
    if (!currentOwner || !currentOwner.Account.AccountID || !this.currentUserAccounts || this.currentUserAccounts.length == 0) {
      return null;
    }
    
    let account = this.currentUserAccounts.filter(x => x.AccountID == currentOwner.Account.AccountID)[0];

    if (!account) {
      return null;
    }
    
    return account.AccountNumber
  }

  public isActive() : boolean {
    if (!this.parcel) {
      return false;
    }

    let currentOwner = this.getCurrentOwner();

    return currentOwner != null && currentOwner != undefined;
  }
  
  public isAdministrator() : boolean
  {
    return this.authenticationService.isCurrentUserAnAdministrator();
  }

  public isDemoUser() : boolean
  {
    return this.authenticationService.isCurrentUserADemoUser();
  }

  public isLandowner(): boolean
  {
    return this.authenticationService.isCurrentUserALandOwner();
  }

  public showGoToDashboardButton(): boolean {
    let currentOwner = this.getCurrentOwner();
    return this.currentUser && this.currentUserAccounts && this.currentUserAccounts.length > 0 && currentOwner && currentOwner.Account && this.currentUserAccounts.some(x => x.AccountID == currentOwner.Account.AccountID);
  }
}
