import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { OAuthModule, OAuthService } from 'angular-oauth2-oidc';
import { CookieModule } from 'ngx-cookie';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './shared/interceptors/auth-interceptor';
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { HomeIndexComponent } from './pages/home/home-index/home-index.component';
import { UserListComponent } from './pages/user-list/user-list.component';
import { AlertDisplayComponent } from './shared/components/alert-display/alert-display.component';
import { RouterModule } from '@angular/router';
import { UserInviteComponent } from './pages/user-invite/user-invite.component';
import { UserDetailComponent } from './pages/user-detail/user-detail.component';
import { MarketMetricsHomeComponent } from './pages/market-metrics-home/market-metrics-home.component';
import { ManagerDashboardComponent } from './pages/manager-dashboard/manager-dashboard.component';
import { UserEditComponent } from './pages/user-edit/user-edit.component';
import { PostingListComponent } from './pages/posting-list/posting-list.component';
import { PostingNewComponent } from './pages/posting-new/posting-new.component';
import { PostingDetailComponent } from './pages/posting-detail/posting-detail.component';
import { ParcelDetailComponent } from './pages/parcel-detail/parcel-detail.component';
import { ParcelEditAllocationComponent } from './pages/parcel-edit-allocation/parcel-edit-allocation.component';
import { LandownerDashboardComponent } from './pages/landowner-dashboard/landowner-dashboard.component';
import { TradeDetailComponent } from './pages/trade-detail/trade-detail.component';
import { RegisterTransferComponent } from './pages/register-transfer/register-transfer.component';
import { AgGridModule } from 'ag-grid-angular';
import { ParcelListComponent } from './pages/parcel-list/parcel-list.component';
import { DecimalPipe, CurrencyPipe, DatePipe } from '@angular/common';
import { LinkRendererComponent } from './shared/components/ag-grid/link-renderer/link-renderer.component';


import { FormsModule } from '@angular/forms';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LandownerWaterUseChartComponent } from './pages/landowner-water-use-chart/landowner-water-use-chart.component';
import { AllocationChartImplComponent } from './pages/landowner-water-allocation-chart/allocation-chart-impl.component';
import { ComboSeriesVerticalComponent } from './shared/components/combo-chart/combo-series-vertical.component'
import { LandownerWaterAllocationChartComponent } from './pages/landowner-water-allocation-chart/landowner-water-allocation-chart.component';
import { FontAwesomeIconLinkRendererComponent } from './shared/components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { PostingDeleteComponent } from './pages/posting-delete/posting-delete.component';
import { LoginCallbackComponent } from './pages/login-callback/login-callback.component';
import { HelpComponent } from './pages/help/help.component';
import { ParcelBulkSetAllocationComponent } from './pages/parcel-bulk-set-allocation/parcel-bulk-set-allocation.component';
import { GlossaryComponent } from './pages/glossary/glossary.component';
import { ParcelChangeOwnerComponent } from './pages/parcel-change-owner/parcel-change-owner.component';
import { SelectDropDownModule } from 'ngx-select-dropdown'
import { MyDatePickerModule } from 'mydatepicker';
import { ParcelOverrideEtDataComponent } from './pages/parcel-override-et-data/parcel-override-et-data.component';
import { AccountListComponent } from './pages/account-list/account-list.component';
import { AccountDetailComponent } from './pages/account-detail/account-detail.component';
import { AccountEditComponent } from './pages/account-edit/account-edit.component';
import { AccountNewComponent } from './pages/account-new/account-new.component';
import { AccountEditUsersComponent } from './pages/account-edit-users/account-edit-users.component';
import { MultiLinkRendererComponent } from './shared/components/ag-grid/multi-link-renderer/multi-link-renderer.component';
import { CreateUserCallbackComponent } from './pages/create-user-callback/create-user-callback.component';
import { UserEditAccountsComponent } from './pages/user-edit-accounts/user-edit-accounts.component';
import { AboutComponent } from './pages/about/about.component';
import { GeneralFaqComponent } from './pages/general-faq/general-faq.component';
import { WaterUseMeasurementComponent } from './pages/measuring-water-use-with-openet/measuring-water-use-with-openet.component';
import { DisclaimerComponent } from './pages/disclaimer/disclaimer.component';

@NgModule({
  declarations: [
    AlertDisplayComponent,
    AppComponent,
    HomeIndexComponent,
    UserListComponent,
    UserInviteComponent,
    UserDetailComponent,
    MarketMetricsHomeComponent,
    ManagerDashboardComponent,
    UserEditComponent,
    PostingListComponent,
    PostingNewComponent,
    PostingDetailComponent,
    ParcelDetailComponent,
    ParcelEditAllocationComponent,
    LandownerDashboardComponent,
    TradeDetailComponent,
    RegisterTransferComponent,
    ParcelListComponent,
    LandownerWaterUseChartComponent,
    AllocationChartImplComponent,
    ComboSeriesVerticalComponent,
    LandownerWaterAllocationChartComponent,
    PostingDeleteComponent,
    LoginCallbackComponent,
    HelpComponent,
    ParcelBulkSetAllocationComponent,
    GlossaryComponent,
    ParcelChangeOwnerComponent,
    ParcelOverrideEtDataComponent,
    AccountListComponent,
    AccountDetailComponent,
    AccountEditComponent,
    AccountNewComponent,
    AccountEditUsersComponent,
    CreateUserCallbackComponent,
    UserEditAccountsComponent,
    AboutComponent,
    GeneralFaqComponent,
    WaterUseMeasurementComponent,
    DisclaimerComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    NgbModule,
    RouterModule,
    OAuthModule.forRoot(),
    CookieModule.forRoot(),
    SharedModule.forRoot(),
    FormsModule,
    NgxChartsModule,
    BrowserAnimationsModule,
    AgGridModule.withComponents([]),
    SelectDropDownModule,
    MyDatePickerModule
  ],  
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    DecimalPipe, CurrencyPipe, DatePipe
  ],
  entryComponents: [LinkRendererComponent, FontAwesomeIconLinkRendererComponent, MultiLinkRendererComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
