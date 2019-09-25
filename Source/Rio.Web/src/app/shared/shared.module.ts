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
import { LinkRendererComponent } from './components/ag-grid/link-renderer/link-renderer.component';
import { FontAwesomeIconLinkRendererComponent } from './components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { ParcelPickerComponent } from './components/parcel-picker/parcel-picker.component';
import { ParcelMapComponent } from './components/parcel-map/parcel-map.component';

@NgModule({
    declarations: [
        HeaderNavComponent,
        FooterComponent,        
        NotFoundComponent,
        UnauthenticatedComponent,
        SubscriptionInsufficientComponent,
        ParcelMapComponent,
        ParcelDetailPopupComponent,
        LinkRendererComponent,
        FontAwesomeIconLinkRendererComponent,
        ParcelPickerComponent
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
        ParcelMapComponent,
        ParcelPickerComponent,
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
