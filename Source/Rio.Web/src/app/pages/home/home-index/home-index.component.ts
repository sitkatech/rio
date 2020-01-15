import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { UserDto } from 'src/app/shared/models';
import { error } from 'protractor';
import { RoleEnum } from 'src/app/shared/models/enums/role.enum';

@Component({
    selector: 'app-home-index',
    templateUrl: './home-index.component.html',
    styleUrls: ['./home-index.component.scss']
})
export class HomeIndexComponent implements OnInit, OnDestroy {
    public watchUserChangeSubscription: any;
    public currentUser: UserDto;

    constructor(private authenticationService: AuthenticationService) {
    }

    public ngOnInit(): void {
        this.watchUserChangeSubscription = this.authenticationService.currentUserSetObservable.subscribe(currentUser => { 
            this.currentUser = currentUser;
        });
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

    public isUserAnAdministrator(){
        return this.authenticationService.isUserAnAdministrator(this.currentUser);
    }

    public isUserALandowner(){
        return this.authenticationService.isUserALandOwner(this.currentUser);
    }

    public login(): void {
        this.authenticationService.login();
    }

    public createAccount(): void{
        this.authenticationService.createAccount();
    }
}
