export class AccountUpdateDto {
    Notes: string;
    AccountName: string;
    AccountStatusID: number;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
