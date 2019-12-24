
export class AccountSimpleDto{
    AccountID: number;
    AccountName: string;
    AccountNumber: number
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

