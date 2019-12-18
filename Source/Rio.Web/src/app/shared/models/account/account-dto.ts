import { UserSimpleDto } from '../user/user-simple-dto';

export class AccountDto{
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    Users: Array<UserSimpleDto>;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}