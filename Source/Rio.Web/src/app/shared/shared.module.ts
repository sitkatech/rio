import { NgModule, ModuleWithProviders } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NotFoundComponent } from './pages';
import { HeaderNavComponent } from './components';
import { UnauthenticatedComponent } from './pages/unauthenticated/unauthenticated.component';
import { SubscriptionInsufficientComponent } from './pages/subscription-insufficient/subscription-insufficient.component';
import { NgProgressModule } from 'ngx-progressbar';
import { RouterModule } from '@angular/router';
import { ParcelDetailPopupComponent } from './components/parcel-detail-popup/parcel-detail-popup.component';
import { LinkRendererComponent } from './components/ag-grid/link-renderer/link-renderer.component';
import { FontAwesomeIconLinkRendererComponent } from './components/ag-grid/fontawesome-icon-link-renderer/fontawesome-icon-link-renderer.component';
import { ParcelPickerComponent } from './components/parcel-picker/parcel-picker.component';
import { ParcelMapComponent } from './components/parcel-map/parcel-map.component';
import { MultiLinkRendererComponent } from './components/ag-grid/multi-link-renderer/multi-link-renderer.component';
import { SelectDropDownModule } from 'ngx-select-dropdown';
import { WaterYearSelectComponent } from './components/water-year-select/water-year-select/water-year-select.component';
import { ScenarioMapComponent } from './components/scenario-map/scenario-map.component';
import { CustomRichTextComponent } from './components/custom-rich-text/custom-rich-text.component';
import { CKEditorModule } from '@ckeditor/ckeditor5-angular';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CheckboxRendererComponent } from './components/ag-grid/checkbox-renderer/checkbox-renderer.component';

@NgModule({
    declarations: [
        HeaderNavComponent,
        NotFoundComponent,
        UnauthenticatedComponent,
        SubscriptionInsufficientComponent,
        ParcelMapComponent,
        ParcelDetailPopupComponent,
        LinkRendererComponent,
        FontAwesomeIconLinkRendererComponent,
        ParcelPickerComponent,
        MultiLinkRendererComponent,
        WaterYearSelectComponent,
        ScenarioMapComponent,
        CustomRichTextComponent,
        CheckboxRendererComponent
    ],
    imports: [
        CommonModule,
        FormsModule,
        HttpClientModule,
        NgProgressModule,
        RouterModule,
        SelectDropDownModule,
        CKEditorModule,
        NgbModule
    ],
    exports: [
        CommonModule,
        FormsModule,
        NotFoundComponent,
        ParcelMapComponent,
        ParcelPickerComponent,
        HeaderNavComponent,
        WaterYearSelectComponent,
        ScenarioMapComponent,
        CustomRichTextComponent
    ],
    entryComponents:[
        ParcelDetailPopupComponent
    ]
})
export class SharedModule {
    static forRoot(): ModuleWithProviders<SharedModule> {
    return {
        ngModule: SharedModule,
        providers: []
    };
}

    static forChild(): ModuleWithProviders<SharedModule> {
    return {
        ngModule: SharedModule,
        providers: []
    };
}
}
