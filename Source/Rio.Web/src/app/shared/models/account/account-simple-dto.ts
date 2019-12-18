
export class AccountSimpleDto{
    AccountName: string;
    AccountNumber: number;
    Notes: string;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

