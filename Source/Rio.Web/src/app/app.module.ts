import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { SharedModule } from './shared/shared.module';
import { OAuthModule } from 'angular-oauth2-oidc';
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
import { TradesHomeComponent } from './pages/trades-home/trades-home.component';
import { MarketMetricsHomeComponent } from './pages/market-metrics-home/market-metrics-home.component';
import { ParcelsHomeComponent } from './pages/parcels-home/parcels-home.component';
import { ParcelMapComponent } from './shared/components/parcel-map/parcel-map.component';
import { UserEditComponent } from './pages/user-edit/user-edit.component';

@NgModule({
  declarations: [
    AlertDisplayComponent,
    AppComponent,
    HomeIndexComponent,
    UserListComponent,
    UserInviteComponent,
    UserDetailComponent,
    TradesHomeComponent,
    MarketMetricsHomeComponent,
    ParcelsHomeComponent,
    ParcelMapComponent,
    UserEditComponent
  ],
  imports: [
    AppRoutingModule,
    BrowserModule,
    BrowserAnimationsModule,
    NgbModule,
    RouterModule,
    OAuthModule.forRoot(),
    CookieModule.forRoot(),
    SharedModule.forRoot()
  ],  
  providers: [
    { provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true },
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
