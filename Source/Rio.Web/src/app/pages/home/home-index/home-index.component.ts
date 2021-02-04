import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { error } from 'protractor';
import { RoleEnum } from 'src/app/shared/models/enums/role.enum';
import { environment } from 'src/environments/environment';
import { CustomRichTextType } from 'src/app/shared/models/enums/custom-rich-text-type.enum';
import { ApplicationInternalNameEnum } from 'src/app/shared/models/enums/application-internal-name.enum';
import { AccountSimpleDto } from 'src/app/shared/models/account/account-simple-dto';

@Component({
    selector: 'app-home-index',
    templateUrl: './home-index.component.html',
    styleUrls: ['./home-index.component.scss']
})
export class HomeIndexComponent implements OnInit, OnDestroy {
    public watchUserChangeSubscription: any;
    public currentUser: UserDto;
    public homepageRichTextTypeID: number = CustomRichTextType.HomePage;
    public applicationInternalName: string = environment.applicationInternalName;
    public ApplicationInternalNameEnum = ApplicationInternalNameEnum;
    public currentUserAccounts: AccountSimpleDto[];

    constructor(private authenticationService: AuthenticationService) {
    }

    public ngOnInit(): void {
        if (localStorage.getItem("loginOnReturn")){
            localStorage.removeItem("loginOnReturn");
            this.authenticationService.login();
        }
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
            this.currentUser = currentUser;

            if (currentUser && !this.userIsUnassigned() && !this.userRoleIsDisabled()) {
                this.currentUserAccounts = this.authenticationService.getAvailableAccounts();
            }
        });

        console.log(environment);
    }

    ngOnDestroy(): void {
      this.watchUserChangeSubscription.unsubscribe();
    }
    
    public userIsUnassigned(){
        if (!this.currentUser){
            return false; // doesn't exist != unassigned
        }
        
        return this.currentUser.Role.RoleID === RoleEnum.Unassigned;
    }

    public userRoleIsDisabled(){
        if (!this.currentUser){
            return false; // doesn't exist != unassigned
        }
        
        return this.currentUser.Role.RoleID === RoleEnum.Disabled;
    }

    public isUserAnAdministrator(){
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public isUserALandowner(){
        return this.authenticationService.isUserALandOwnerOrDemoUser(this.currentUser);
    }

    public login(): void {
        this.authenticationService.login();
    }

    public createAccount(): void{
        this.authenticationService.createAccount();
    }

    public forgotPasswordUrl() :string{
        return `${environment.keystoneSupportBaseUrl}/ForgotPassword`;
    }

    public forgotUsernameUrl() :string{
        return `${environment.keystoneSupportBaseUrl}/ForgotUsername`;
    }

    public keystoneSupportUrl():string{
        return `${environment.keystoneSupportBaseUrl}/Support/20`;
    }

    public platformLongName():string{
        return environment.platformLongName;
    }

    public platformShortName():string{
        return environment.platformShortName;
    }

    public leadOrganizationShortName():string{
        return environment.leadOrganizationShortName;
    }

    public leadOrganizationLongName(): string{
        return environment.leadOrganizationLongName;
    }

    public leadOrganizationHomeUrl(): string{
        return environment.leadOrganizationHomeUrl;
    }

    public applicationType(): string {
        return environment.applicationType;
    }
}
