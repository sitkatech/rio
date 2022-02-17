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
import { TransactionTypeEnum } from 'src/app/shared/models/enums/transaction-type-enum';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { ParcelLedgerEntrySourceTypeEnum } from 'src/app/shared/models/enums/parcel-ledger-entry-source-type-enum';
import { environment } from 'src/environments/environment';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';

@Component({
  selector: 'template-parcel-detail',
  templateUrl: './parcel-detail.component.html',
  styleUrls: ['./parcel-detail.component.scss']
})
export class ParcelDetailComponent implements OnInit, OnDestroy {
  @ViewChild('parcelLedgerGrid') parcelLedgerGrid: AgGridAngular;
  
  private currentUser: UserDto;
  private currentUserAccounts: AccountSimpleDto[];

  public waterYears: Array<WaterYearDto>;
  public parcel: ParcelDto;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public waterSupplyParcelLedgers: Array<ParcelLedgerDto>;
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
  ) {
    // force route reload whenever params change;
    this.router.routeReuseStrategy.shouldReuseRoute = () => false;
  }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
      const id = parseInt(this.route.snapshot.paramMap.get("id"));
      this.initializeLedgerGrid();
      if (id) {
        forkJoin(
          this.parcelService.getParcelWithTagsByParcelID(id),
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
          this.waterSupplyParcelLedgers = this.getWaterSupplyParcelLedgers(parcelLedgers);
          this.usageParcelLedgers = parcelLedgers.filter(x => x.TransactionType.TransactionTypeID == TransactionTypeEnum.Usage);
          this.waterYears = waterYears;
          this.parcelOwnershipHistory = parcelOwnershipHistory;
          this.waterTypes = waterTypes;
        });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
      
    });
  }

  initializeLedgerGrid() {
    this.parcelLedgerGridColumnDefs = [
      this.utilityFunctionsService.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy'),
      this.utilityFunctionsService.createDateColumnDef('Transaction Date', 'TransactionDate', 'short'),
      { headerName: 'Transaction Type', field: 'TransactionType.TransactionTypeName' },
      {
        headerName: 'Water Type', valueGetter: function (params: any) {
          return params.data.WaterType ? params.data.WaterType.WaterTypeName : '-';
        }
      },
      { headerName: 'Source Type', field: 'ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName' },
      this.utilityFunctionsService.createDecimalColumnDef('Transaction Volume (ac-ft)', 'TransactionAmount'),
      this.utilityFunctionsService.createDecimalColumnDef('Transaction Depth (ac-ft / ac)', 'TransactionDepth'),
      { headerName: 'Transaction Description', field: 'TransactionDescription', sortable: false },
      { 
        headerName: 'Comment', field: 'UserComment', filter: false, sortable: false,
        valueGetter: params => params.data.UserComment ?? '-'
      }
    ];

    this.defaultColDef = {
      resizable: true,
      filter: true,
      sortable: true
    };

  }

  public exportParcelLedgerGridToCsv() {
    this.utilityFunctionsService.exportGridToCsv(this.parcelLedgerGrid, 'parcelLedgerfor' + this.parcel.ParcelNumber + '.csv', null);
  }
  
  private onGridReady(params) {
    this.gridColumnApi = params.columnApi;

    this.gridColumnApi.autoSizeAllColumns();
  }

  ngOnDestroy() {
    this.cdr.detach();
  }

  public allowTrading(): boolean {
    return environment.allowTrading;
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }

  public removeTag(tag: TagDto) {
    const tagIndex = this.parcel.Tags.indexOf(tag);

    this.parcelService.removeTagFromParcel(this.parcel.ParcelID, tag.TagID).subscribe(() => {
      this.parcel.Tags.splice(tagIndex, 1);
    }, error => {
      window.scroll(0, 0);
    });
  }

  public getWaterSupplyParcelLedgers(parcelLedgersForWaterYear: Array<ParcelLedgerDto>): Array<ParcelLedgerDto> {
    const supplyEntrySourceTypeIDs = [ParcelLedgerEntrySourceTypeEnum.Manual, ParcelLedgerEntrySourceTypeEnum.CIMIS, ParcelLedgerEntrySourceTypeEnum.Trade];
    return parcelLedgersForWaterYear.filter(x => 
      x.TransactionType.TransactionTypeID == TransactionTypeEnum.Supply && 
      supplyEntrySourceTypeIDs.indexOf(x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID) > -1
    );
  }
  
  public getTotalWaterSupplyForYear(year: number): string {
    var parcelLedgerForYear = this.waterSupplyParcelLedgers.filter(x => x.WaterYear === year);
    if (parcelLedgerForYear.length === 0) {
      return "-";
    }
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgerForYear).toFixed(1);
  }

  public getWaterSupplyForYearByType(waterType: WaterTypeDto, year: number): string {
    var parcelLedgers = this.waterSupplyParcelLedgers.filter(x => x.WaterYear === year && x.WaterType != null && x.WaterType.WaterTypeID === waterType.WaterTypeID);
    if (parcelLedgers.length === 0) {
      return "-";
    }
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers).toFixed(1);
  }

  public getPrecipWaterSupplyForYear(year: number): string {
    var parcelLedgers = this.parcelLedgers.filter(x => x.WaterYear === year && 
      x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.CIMIS);
    
      if (parcelLedgers.length === 0) {
      return "-";
    }
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers).toFixed(1);
  }

  public getPurchasedWaterSupplyForYear(year: number): string {
    var parcelLedgers = this.parcelLedgers.filter(x => x.WaterYear === year && 
      x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.Trade &&
      x.TransactionAmount > 0);
    
    if (parcelLedgers.length === 0) {
      return "-";
    }
    return this.getTotalTransactionAmountForParcelLedgers(parcelLedgers).toFixed(1);
  }

  public getSoldWaterSupplyForYear(year: number): string {
    var parcelLedgers = this.parcelLedgers.filter(x => x.WaterYear === year && 
      x.ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeID === ParcelLedgerEntrySourceTypeEnum.Trade &&
      x.TransactionAmount < 0);
    
    if (parcelLedgers.length === 0) {
      return "-";
    }
    return Math.abs(this.getTotalTransactionAmountForParcelLedgers(parcelLedgers)).toFixed(1);
  }

  public getConsumptionForYear(year: number): string {
    let parcelLedgersForYear = this.usageParcelLedgers.filter(x => x.WaterYear == year);
    
    if (parcelLedgersForYear.length === 0) {
      return "-";
    }
    let monthlyUsage = this.getTotalTransactionAmountForParcelLedgers(parcelLedgersForYear);
    
    return Math.abs(monthlyUsage).toFixed(1);
  }

  public getTotalTransactionAmountForParcelLedgers(parcelLedgers: Array<ParcelLedgerDto>): number {
    return parcelLedgers.reduce((a, b) => {
      return a + b.TransactionAmount;
    }, 0);
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
