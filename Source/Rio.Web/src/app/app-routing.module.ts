import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent, UnauthenticatedComponent, SubscriptionInsufficientComponent } from './shared/pages';
import { UnauthenticatedAccessGuard } from './shared/guards/unauthenticated-access/unauthenticated-access.guard';
import { AllowTradeGuard } from "./shared/guards/unauthenticated-access/allow-trade-guard";
import { ManagerOnlyGuard } from "./shared/guards/unauthenticated-access/manager-only-guard";
import { AcknowledgedDisclaimerGuard } from "./shared/guards/acknowledged-disclaimer-guard";
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
import { PostingDeleteComponent } from './pages/posting-delete/posting-delete.component';
import { LoginCallbackComponent } from './pages/login-callback/login-callback.component';
import { HelpComponent } from './pages/help/help.component';
import { SetWaterAllocationComponent } from './pages/set-water-allocation/set-water-allocation.component';
import { GlossaryComponent } from './pages/glossary/glossary.component';
import { ParcelChangeOwnerComponent } from './pages/parcel-change-owner/parcel-change-owner.component';
import { ParcelOverrideEtDataComponent } from './pages/parcel-override-et-data/parcel-override-et-data.component';
import { AccountListComponent } from './pages/account-list/account-list.component';
import { AccountDetailComponent } from './pages/account-detail/account-detail.component';
import { AccountEditComponent } from './pages/account-edit/account-edit.component';
import { AccountNewComponent } from './pages/account-new/account-new.component';
import { AccountEditUsersComponent } from './pages/account-edit-users/account-edit-users.component';
import { CreateUserCallbackComponent } from './pages/create-user-callback/create-user-callback.component';
import { UserEditAccountsComponent } from './pages/user-edit-accounts/user-edit-accounts.component';
import { AboutComponent } from './pages/about/about.component';
import { GeneralFaqComponent } from './pages/general-faq/general-faq.component';
import { WaterUseMeasurementComponent } from './pages/measuring-water-use-with-openet/measuring-water-use-with-openet.component';
import { DisclaimerComponent } from './pages/disclaimer/disclaimer.component';
import { AboutGroundwaterEvaluationComponent } from './pages/about-groundwater-evaluation/about-groundwater-evaluation.component';
import { ManagedRechargeScenarioComponent } from './pages/managed-recharge-scenario/managed-recharge-scenario.component';
import { WaterTradingScenarioComponent } from './pages/water-trading-scenario/water-trading-scenario.component';
import { GETIntegrationEnabledGuard } from './shared/guards/unauthenticated-access/GET-integration-enabled-guard';
import { ManagerOrDemoUserOnlyGuard } from './shared/guards/unauthenticated-access/manager-or-demouser-only-guard';
import { RolesAndPermissionsComponent } from './pages/roles-and-permissions/roles-and-permissions.component';
import { ParcelAllocationTypeEditComponent } from './pages/parcel-allocation-type-edit/parcel-allocation-type-edit.component';
import { TrainingVideosComponent } from './pages/training-videos/training-videos.component';

const routes: Routes = [
  { path: "trades", component: PostingListComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "trades/:tradeNumber", component: TradeDetailComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "register-transfer/:waterTransferID", component: RegisterTransferComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "new-posting", component: PostingNewComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "postings/:postingID", component: PostingDetailComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "delete-posting/:postingID", component: PostingDeleteComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, AcknowledgedDisclaimerGuard] },
  { path: "market-metrics", component: MarketMetricsHomeComponent, canActivate: [UnauthenticatedAccessGuard, AllowTradeGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcel-allocation-types/edit", component: ParcelAllocationTypeEditComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "parcels", component: ParcelListComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcels/set-water-allocation", component: SetWaterAllocationComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcels/:id", component: ParcelDetailComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcels/:id/edit-annual-allocation", component: ParcelEditAllocationComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcels/:id/change-owner", component: ParcelChangeOwnerComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "users", component: UserListComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "users/:id", component: UserDetailComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "users/:id/edit", component: UserEditComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "users/:id/edit-accounts", component: UserEditAccountsComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "accounts", component: AccountListComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "accounts/:id", component: AccountDetailComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "accounts/:id/edit", component: AccountEditComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "accounts/:id/edit-users", component: AccountEditUsersComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "new-account", component: AccountNewComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard]},
  { path: "landowner-dashboard/:id", component: LandownerDashboardComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "landowner-dashboard", component: LandownerDashboardComponent, canActivate: [UnauthenticatedAccessGuard, AcknowledgedDisclaimerGuard] },
  { path: "parcel-override-et-data/:accountID/:waterYear", component: ParcelOverrideEtDataComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "manager-dashboard", component: ManagerDashboardComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOrDemoUserOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "invite-user/:userID", component: UserInviteComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "invite-user", component: UserInviteComponent, canActivate: [UnauthenticatedAccessGuard, ManagerOnlyGuard, AcknowledgedDisclaimerGuard] },
  { path: "about-groundwater-evaluation", component: AboutGroundwaterEvaluationComponent, canActivate: [GETIntegrationEnabledGuard]},
  { path: "managed-recharge-scenario", component: ManagedRechargeScenarioComponent, canActivate: [GETIntegrationEnabledGuard]},
  { path: "water-trading-scenario", component: WaterTradingScenarioComponent, canActivate: [GETIntegrationEnabledGuard]},
  { path: "training-videos", component: TrainingVideosComponent},
  { path: "", component: HomeIndexComponent},
  { path: "disclaimer", component: DisclaimerComponent },
  { path: "disclaimer/:forced", component: DisclaimerComponent },
  { path: "help", component: HelpComponent },
  { path: "platform-overview", component: AboutComponent},
  { path: "frequently-asked-questions", component: GeneralFaqComponent},
  { path: "measuring-water-use-with-openet", component: WaterUseMeasurementComponent},
  { path: "glossary", component: GlossaryComponent },
  { path: "roles-and-permissions", component: RolesAndPermissionsComponent },
  { path: "login-callback", component: LoginCallbackComponent },
  { path: "create-user-callback", component: CreateUserCallbackComponent },
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
