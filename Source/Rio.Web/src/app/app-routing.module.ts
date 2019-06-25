import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent, UnauthenticatedComponent, SubscriptionInsufficientComponent } from './shared/pages';
import { UnauthenticatedAccessGuard } from './shared/guards/unauthenticated-access/unauthenticated-access.guard';
import { UserListComponent } from './pages/user-list/user-list.component';
import { HomeIndexComponent } from './pages/home/home-index/home-index.component';
import { UserDetailComponent } from './pages/user-detail/user-detail.component';
import { UserInviteComponent } from './pages/user-invite/user-invite.component';
import { MarketMetricsHomeComponent } from './pages/market-metrics-home/market-metrics-home.component';
import { TradesHomeComponent } from './pages/trades-home/trades-home.component';
import { ParcelsHomeComponent } from './pages/parcels-home/parcels-home.component';

const routes: Routes = [
  { path: "trades", component: TradesHomeComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "market-metrics", component: MarketMetricsHomeComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "parcels", component: ParcelsHomeComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "users", component: UserListComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "users/:id", component: UserDetailComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "invite-user", component: UserInviteComponent, canActivate: [UnauthenticatedAccessGuard] },
  // { path: "users/:id/edit-permissions", component: PersonEditPermissionsComponent, canActivate: [AuthGuard] },
  { path: "", component: HomeIndexComponent },
  { path: "not-found", component: NotFoundComponent },
  { path: 'subscription-insufficient', component: SubscriptionInsufficientComponent },
  { path: 'unauthenticated', component: UnauthenticatedComponent },
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
