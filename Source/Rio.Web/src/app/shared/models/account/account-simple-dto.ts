
export class AccountSimpleDto{
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

