import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { NotFoundComponent } from './pages';
import { HeaderNavComponent, FooterComponent, UserAuthenticationComponent } from './components';
import { UnauthenticatedComponent } from './pages/unauthenticated/unauthenticated.component';
import { SubscriptionInsufficientComponent } from './pages/subscription-insufficient/subscription-insufficient.component';
import { NgProgressModule } from '@ngx-progressbar/core';
import { RouterModule } from '@angular/router';

@NgModule({
    declarations: [
        HeaderNavComponent,
        FooterComponent,
        UserAuthenticationComponent,
        NotFoundComponent,
        UnauthenticatedComponent,
        SubscriptionInsufficientComponent
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
