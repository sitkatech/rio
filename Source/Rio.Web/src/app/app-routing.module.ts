import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent, UnauthenticatedComponent, SubscriptionInsufficientComponent } from './shared/pages';
import { UnauthenticatedAccessGuard } from './shared/guards/unauthenticated-access/unauthenticated-access.guard';
import { UserListComponent } from './pages/user-list/user-list.component';
import { HomeIndexComponent } from './pages/home/home-index/home-index.component';
import { UserDetailComponent } from './pages/user-detail/user-detail.component';
import { UserInviteComponent } from './pages/user-invite/user-invite.component';
import { MarketMetricsHomeComponent } from './pages/market-metrics-home/market-metrics-home.component';
import { ManagerDashboardComponent } from './pages/manager-dashboard/manager-dashboard.component';
import { UserEditComponent } from './pages/user-edit/user-edit.component';
import { PostingListComponent } from './pages/posting-list/posting-list.component';
import { PostingDetailComponent } from './pages/posting-detail/posting-detail.component';
import { PostingNewComponent } from './pages/posting-new/posting-new.component';
import { ParcelDetailComponent } from './pages/parcel-detail/parcel-detail.component';
import { ParcelEditAllocationComponent } from './pages/parcel-edit-allocation/parcel-edit-allocation.component';
import { LandownerDashboardComponent } from './pages/landowner-dashboard/landowner-dashboard.component';
import { TradeDetailComponent } from './pages/trade-detail/trade-detail.component';
import { RegisterTransferComponent } from './pages/register-transfer/register-transfer.component';
import { ParcelListComponent } from './pages/parcel-list/parcel-list.component';

const routes: Routes = [
  { path: "trades", component: PostingListComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "trades/:tradeID", component: TradeDetailComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "register-transfer/:waterTransferID", component: RegisterTransferComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "new-posting", component: PostingNewComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "postings/:postingID", component: PostingDetailComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "market-metrics", component: MarketMetricsHomeComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "parcels", component: ParcelListComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "parcels/:id", component: ParcelDetailComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "parcels/:id/edit-annual-allocation", component: ParcelEditAllocationComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "users", component: UserListComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "users/:id", component: UserDetailComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "users/:id/edit", component: UserEditComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "landowner-dashboard/:id", component: LandownerDashboardComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "landowner-dashboard", component: LandownerDashboardComponent },
  { path: "manager-dashboard", component: ManagerDashboardComponent, canActivate: [UnauthenticatedAccessGuard] },
  { path: "invite-user/:userID", component: UserInviteComponent, canActivate: [UnauthenticatedAccessGuard] },
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
