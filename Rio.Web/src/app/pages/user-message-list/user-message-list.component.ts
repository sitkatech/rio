import { DatePipe } from '@angular/common';
import { ChangeDetectorRef, Component, OnInit, ViewChild } from '@angular/core';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef } from 'ag-grid-community';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserService } from 'src/app/services/user/user.service';
import { LinkRendererComponent } from 'src/app/shared/components/ag-grid/link-renderer/link-renderer.component';
import { UserDto } from 'src/app/shared/generated/model/user-dto';
import { UserMessageDto } from 'src/app/shared/generated/model/user-message-dto';

@Component({
  selector: 'fresca-user-message-list',
  templateUrl: './user-message-list.component.html',
  styleUrls: ['./user-message-list.component.scss']
})
export class UserMessageListComponent implements OnInit {
  @ViewChild('userMessagesGrid') userMessagesGrid: AgGridAngular;

  private currentUser: UserDto;

  public rowData = [];
  columnDefs: ColDef[];
  columnDefsUnassigned: ColDef[];
  userMessages: UserMessageDto[];

  constructor(
    private cdr: ChangeDetectorRef,
    private authenticationService: AuthenticationService,
    private userService: UserService,
    private datePipe: DatePipe) { }

  ngOnInit() {
    this.authenticationService.getCurrentUser().subscribe(currentUser => {
      this.currentUser = currentUser;
      this.userMessagesGrid?.api.showLoadingOverlay();
      this.userService.getMessagesForLoggedInUser().subscribe(userMessages => {
        this.rowData = userMessages;
        this.userMessagesGrid?.api.hideOverlay();
        this.userMessages = userMessages;
        this.cdr.detectChanges();
      });

      let _datePipe = this.datePipe;
      this.columnDefs = [
        {
          headerName: '', valueGetter: function (params: any) {
            return { LinkValue: params.data.UserMessageID, LinkDisplay: "View", CssClasses: "btn btn-sm btn-primary" };
          }, cellRendererFramework: LinkRendererComponent,
          cellRendererParams: { inRouterLink: "/user-messages/" },
          sortable: false, filter: false, width: 80
        },
        {
          headerName: 'From', field: 'CreateUserID',
          valueGetter: function (params: any) {
            return params.data.CreateUser.FullName;
          },
          filterValueGetter: function (params: any) {
            return params.data.CreateUser.FullName;
          },
          comparator: function (id1: any, id2: any) {
            let link1 = id1.LinkDisplay;
            let link2 = id2.LinkDisplay;
            if (link1 < link2) {
              return -1;
            }
            if (link1 > link2) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true, width: 170
        },
        {
          headerName: 'To', field: 'RecipientUserID',
          valueGetter: function (params: any) {
            return params.data.RecipientUser.FullName;
          },
          filterValueGetter: function (params: any) {
            return params.data.RecipientUser.FullName;
          },
          comparator: function (id1: any, id2: any) {
            let link1 = id1.LinkDisplay;
            let link2 = id2.LinkDisplay;
            if (link1 < link2) {
              return -1;
            }
            if (link1 > link2) {
              return 1;
            }
            return 0;
          },
          sortable: true, filter: true, width: 170
        },
        {
          headerName: 'Created Date', field:'CreateDate', valueGetter: function (params: any) {
            return _datePipe.transform(params.data.CreateDate, "short");
          },
          filterValueGetter: function (params: any) {
            return _datePipe.transform(params.data.CreateDate, "M/d/yyyy");
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
        { headerName: 'Message', field: 'Message', sortable: true, filter: true, width: 575 },
      ];

      this.columnDefs.forEach(x => {
        x.resizable = true;
      });
    });
  }

  ngOnDestroy() {
    this.cdr.detach();
  }
}
