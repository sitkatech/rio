import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NotFoundComponent } from './pages';
import { HeaderNavComponent, FooterComponent } from './components';
import { UnauthenticatedComponent } from './pages/unauthenticated/unauthenticated.component';
import { SubscriptionInsufficientComponent } from './pages/subscription-insufficient/subscription-insufficient.component';
import { NgProgressModule } from '@ngx-progressbar/core';
import { RouterModule } from '@angular/router';
import { ParcelDetailPopupComponent } from './components/parcel-detail-popup/parcel-detail-popup.component';
import { ParcelNumberLinkRendererComponent } from './components/ag-grid/parcel-number-link-renderer/parcel-number-link-renderer.component';
import { UserLinkRendererComponent } from './components/ag-grid/user-link-renderer/user-link-renderer.component';
import { TradeDateLinkRendererComponent } from './components/ag-grid/trade-date-link-renderer/trade-date-link-renderer.component';
import { FontAwesomeIconLinkRendererComponent } from './components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';

@NgModule({
    declarations: [
        HeaderNavComponent,
        FooterComponent,        
        NotFoundComponent,
        UnauthenticatedComponent,
        SubscriptionInsufficientComponent,
        ParcelDetailPopupComponent,
        ParcelNumberLinkRendererComponent,
        UserLinkRendererComponent,
        TradeDateLinkRendererComponent,
        FontAwesomeIconLinkRendererComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        NgProgressModule,
        RouterModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        NotFoundComponent,
        HeaderNavComponent
    ],
    entryComponents:[
        ParcelDetailPopupComponent
    ]
})
export class SharedModule {
    static forRoot() {
        return {
            ngModule: SharedModule,
            providers: []
        };
    }

    static forChild() {
        return {
            ngModule: SharedModule,
            providers: []
        };
    }
}
