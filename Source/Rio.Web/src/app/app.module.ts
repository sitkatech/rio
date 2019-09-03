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
import { ParcelMapComponent } from './shared/components/parcel-map/parcel-map.component';
import { UserEditComponent } from './pages/user-edit/user-edit.component';
import { PostingListComponent } from './pages/posting-list/posting-list.component';
import { PostingNewComponent } from './pages/posting-new/posting-new.component';
import { PostingDetailComponent } from './pages/posting-detail/posting-detail.component';
import { PostingEditComponent } from './pages/posting-edit/posting-edit.component';
import { ParcelDetailComponent } from './pages/parcel-detail/parcel-detail.component';
import { ParcelEditAllocationComponent } from './pages/parcel-edit-allocation/parcel-edit-allocation.component';
import { LandownerDashboardComponent } from './pages/landowner-dashboard/landowner-dashboard.component';
import { TradeDetailComponent } from './pages/trade-detail/trade-detail.component';
import { ConfirmTransferComponent } from './pages/confirm-transfer/confirm-transfer.component';
import { AgGridModule } from 'ag-grid-angular';
import { ParcelListComponent } from './pages/parcel-list/parcel-list.component';
import { DecimalPipe } from '@angular/common';
import { LinkRendererComponent } from './shared/components/ag-grid/link-renderer/link-renderer.component';
import { LandOwnerLinkRendererComponent } from './shared/components/ag-grid/land-owner-link-renderer/land-owner-link-renderer.component';


import { FormsModule } from '@angular/forms';
import { NgxChartsModule } from '@swimlane/ngx-charts';
import { LandownerWaterUseChartComponent } from './pages/landowner-water-use-chart/landowner-water-use-chart.component';

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
    ParcelMapComponent,
    UserEditComponent,
    PostingListComponent,
    PostingNewComponent,
    PostingDetailComponent,
    PostingEditComponent,
    ParcelDetailComponent,
    ParcelEditAllocationComponent,
    LandownerDashboardComponent,
    TradeDetailComponent,
    ConfirmTransferComponent,
    ParcelListComponent,
    LandownerWaterUseChartComponent,
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
    AgGridModule.withComponents([])
  ],  
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
    DecimalPipe
  ],
  entryComponents: [LinkRendererComponent, LandOwnerLinkRendererComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
