export class AccountStatusDto {
    AccountStatusID: number;
    AccountStatusDisplayName: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
