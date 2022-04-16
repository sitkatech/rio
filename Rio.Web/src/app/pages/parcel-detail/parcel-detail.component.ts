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
import { TransactionTypeEnum } from 'src/app/shared/generated/enum/transaction-type-enum';
import { UtilityFunctionsService } from 'src/app/services/utility-functions.service';
import { AccountSimpleDto } from 'src/app/shared/generated/model/account-simple-dto';
import { ParcelLedgerEntrySourceTypeEnum } from 'src/app/shared/generated/enum/parcel-ledger-entry-source-type-enum';
import { environment } from 'src/environments/environment';
import { TagDto } from 'src/app/shared/generated/model/tag-dto';
import { TagService } from 'src/app/services/tag/tag.service';
import { DropdownSelectFilterComponent } from 'src/app/shared/components/ag-grid/dropdown-select-filter/dropdown-select-filter.component';

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
  public tags: Array<TagDto>;
  public parcelLedgers: Array<ParcelLedgerDto>;
  public waterSupplyParcelLedgers: Array<ParcelLedgerDto>;
  public usageParcelLedgers: Array<ParcelLedgerDto>;
  public months: number[];
  public parcelOwnershipHistory: ParcelOwnershipDto[];
  public tagInput: string;
  public isTaggingParcel = false;

  public today: Date = new Date();
  public waterTypes: WaterTypeDto[];

  public parcelLedgerGridColumnDefs: ColDef[];
  public rowData = [];
  public defaultColDef: ColDef;
  public supplyTypeColDefInsertIndex = 3;
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
    private tagService: TagService
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
        forkJoin({
          parcel: this.parcelService.getParcelByParcelID(id),
          parcelLedgers: this.parcelService.getParcelLedgerEntriesByParcelID(id),
          parcelOwnershipHistory: this.parcelService.getParcelOwnershipHistory(id),
          tags: this.tagService.getTagsByParcelID(id),
          waterYears: this.waterYearService.getWaterYears(),
          waterTypes: this.waterTypeService.getWaterTypes()
        }).subscribe(({parcel, parcelLedgers, parcelOwnershipHistory, tags, waterYears, waterTypes}) => {
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
          this.tags = tags;
        });
      }
      this.months = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12];
      
    });
  }

  initializeLedgerGrid() {
    this.parcelLedgerGridColumnDefs = [
      this.utilityFunctionsService.createDateColumnDef('Effective Date', 'EffectiveDate', 'M/d/yyyy'),
      this.utilityFunctionsService.createDateColumnDef('Transaction Date', 'TransactionDate', 'short'),
      { 
        headerName: 'Transaction Type', field: 'TransactionType.TransactionTypeName',
        filterFramework: DropdownSelectFilterComponent, filterParams: { field: 'TransactionType.TransactionTypeName' }
      },
      { 
        headerName: 'Source Type', field: 'ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName',
        filterFramework: DropdownSelectFilterComponent, filterParams: { field: 'ParcelLedgerEntrySourceType.ParcelLedgerEntrySourceTypeDisplayName' }
      },
      this.utilityFunctionsService.createDecimalColumnDef('Transaction Volume (ac-ft)', 'TransactionAmount'),
      this.utilityFunctionsService.createDecimalColumnDef('Transaction Depth (ac-ft / ac)', 'TransactionDepth'),
      { headerName: 'Transaction Description', field: 'TransactionDescription', sortable: false },
      { 
        headerName: 'Comment', field: 'UserComment', filter: false, sortable: false,
        valueGetter: params => params.data.UserComment ?? '-'
      }
    ];

    if (this.includeWaterSupply()) {
      const waterTypeColDef: ColDef = {
        headerName: 'Supply Type', valueGetter: params => params.data.WaterType ? params.data.WaterType.WaterTypeName : '-',
        filterFramework: DropdownSelectFilterComponent, filterParams: { field: 'WaterType' }
      };
      this.parcelLedgerGridColumnDefs.splice(this.supplyTypeColDefInsertIndex, 0, waterTypeColDef);
    }

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

  public includeWaterSupply():boolean{
    return environment.includeWaterSupply;
  }

  public getSelectedParcelIDs(): Array<number> {
    return this.parcel !== undefined ? [this.parcel.ParcelID] : [];
  }

  public updateTags() {
    this.tagService.getTagsByParcelID(this.parcel.ParcelID).subscribe(tags => {
      this.tags = tags;
    });
  }

  public addTag() {
    if (!this.tagInput) {
      return;
    }
    this.isTaggingParcel = true;
    var newTagDto = new TagDto();
    newTagDto.TagName = this.tagInput;

    this.tagService.tagParcel(this.parcel.ParcelID, newTagDto).subscribe(() => {
      this.isTaggingParcel = false;
      this.updateTags();
      this.tagInput = '';
    }, error => {
      this.isTaggingParcel = false;
      this.tagInput = '';
      window.scroll(0, 0);
    });
  }

  public removeTag(tag: TagDto) {
    this.tagService.removeTagFromParcel(tag.TagID, this.parcel.ParcelID).subscribe(() => {
      this.updateTags();
    }, error => {
      window.scroll(0, 0);
    });
  }

  public getWaterSupplyParcelLedgers(parcelLedgersForWaterYear: Array<ParcelLedgerDto>): Array<ParcelLedgerDto> {
    const supplyEntrySourceTypeIDs = [ParcelLedgerEntrySourceTypeEnum.Manual, ParcelLedgerEntrySourceTypeEnum.Trade];
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
