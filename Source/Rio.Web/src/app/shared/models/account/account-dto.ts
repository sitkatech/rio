import { UserSimpleDto } from '../user/user-simple-dto';
import { AccountStatusDto } from './account-status-dto';
export class AccountDto {
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    AccountStatus: AccountStatusDto;
    Users: Array<UserSimpleDto>;
    AccountDisplayName: string;
    NumberOfParcels: number;
    NumberOfUsers: number;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export class AccountEditUsersDto{
    UserIDs: number[];
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}