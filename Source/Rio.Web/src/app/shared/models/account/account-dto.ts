import { UserSimpleDto } from '../user/user-simple-dto';
import { AccountStatusDto } from './account-status-dto';
export class AccountDto {
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    AccountStatus: AccountStatusDto;
    Users: Array<UserSimpleDto>;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

