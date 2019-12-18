import { UserSimpleDto } from '../user/user-simple-dto';
export class AccountDto {
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    Status: string;
    Users: Array<UserSimpleDto>;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
