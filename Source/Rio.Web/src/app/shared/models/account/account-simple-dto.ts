export class AccountSimpleDto{
    AccountID: number;
    AccountName: string;
    AccountNumber: number;
    AccountVerificationKey: string;
    Notes: string;
    AccountDisplayName: string;
    ShortAccountDisplayName: string;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

