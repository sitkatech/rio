import { Component, OnInit, ChangeDetectorRef, OnDestroy, ViewChild } from '@angular/core';
import { PostingDto } from 'src/app/shared/models/posting/posting-dto';
import { PostingService } from 'src/app/services/posting.service';
import { UserDto } from 'src/app/shared/models';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { PostingTypeEnum } from 'src/app/shared/models/enums/posting-type-enum';
import { ColDef } from 'ag-grid-community';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { DatePipe, CurrencyPipe } from '@angular/common';
import { AgGridAngular } from 'ag-grid-angular';

@Component({
  selector: 'rio-posting-list',
  templateUrl: './posting-list.component.html',
  styleUrls: ['./posting-list.component.scss']
})
export class PostingListComponent implements OnInit, OnDestroy {
  @ViewChild('postingsGrid', {static: false}) postingsGrid: AgGridAngular;

  private watchUserChangeSubscription: any;
  private currentUser: UserDto;

  descriptionMaxLength: number = 300;
  postings: PostingDto[];
  postingToEdit = {};
  columnDefs: ColDef[];
  postingTypeFilter: number;

  constructor(private cdr: ChangeDetectorRef, private authenticationService: AuthenticationService, private postingService: PostingService, private datePipe: DatePipe, private currencyPipe: CurrencyPipe) { }

  ngOnInit() {
    this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => {
      this.postingTypeFilter = 0;
      this.currentUser = currentUser;
      this.postingService.getPostings().subscribe(result => {
        let _datePipe = this.datePipe;
        let _currencyPipe = this.currencyPipe;
        this.postings = result;
        this.columnDefs = [
          {
            headerName: '', valueGetter: function (params: any) {
              return { LinkValue: params.data.PostingID, LinkDisplay: "View Posting", CssClasses: "btn btn-sm btn-primary" };
            }, cellRendererFramework: LinkRendererComponent,
            cellRendererParams: { inRouterLink: "/postings/" },
            sortable: false, filter: false, width: 130
          },
          {
            headerName: 'Posting Date', field: 'PostingDate', valueFormatter: function (params) {
              return _datePipe.transform(params.value, "short")
            },
            filterValueGetter: function (params: any) {
              return _datePipe.transform(params.data.PostingDate, "M/d/yyyy");
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
              if (id1.value < id2.value) {
                return -1;
              }
              if (id1.value > id2.value) {
                return 1;
              }
              return 0;
            },
            sortable: true, filter: 'agDateColumnFilter', width: 140
          },
          { headerName: 'Type', field: 'PostingType.PostingTypeDisplayName', sortable: true, filter: true, width: 100 },
          { headerName: 'Available Quantity', field: 'AvailableQuantity', sortable: true, filter: true, width: 160 },
          { headerName: 'Unit Price (ac-ft)', field: 'Price', valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 140 },
          { headerName: 'Total Price', valueGetter: function (params) { return params.data.Price * params.data.Quantity; }, valueFormatter: function (params) { return _currencyPipe.transform(params.value, "USD"); }, sortable: true, filter: true, width: 130 },
          { headerName: 'Description', field: 'PostingDescription', sortable: true, filter: true },
        ];
      });
    });
  }

  ngOnDestroy() {
    this.watchUserChangeSubscription.unsubscribe();
    this.authenticationService.dispose();
    this.cdr.detach();
  }

  public getPostingsToBuy(): Array<PostingDto> {
    return this.postings ? this.postings.filter(x => x.PostingType.PostingTypeID === PostingTypeEnum.OfferToBuy) : [];
  }

  public getPostingsToSell(): Array<PostingDto> {
    return this.postings ? this.postings.filter(x => x.PostingType.PostingTypeID === PostingTypeEnum.OfferToSell) : [];
  }

  public getAcreFeetToSell(): number {
    return this.getPostingsToSell().reduce(function (a, b) {
      return (a + b.AvailableQuantity);
    }, 0);
  }

  public getAcreFeetToBuy(): number {
    return this.getPostingsToBuy().reduce(function (a, b) {
      return (a + b.AvailableQuantity);
    }, 0);
  }

  public updateGridData(){
    if(this.postingTypeFilter === 0)
    {
      this.postingsGrid.api.setRowData(this.postings);
    }
    else
    {
      let result = this.postings.filter(x => x.PostingType.PostingTypeID === this.postingTypeFilter);
      this.postingsGrid.api.setRowData(result);
    }
  }

}
