export class AccountSimpleDto{
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    AccountDisplayName: string;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

