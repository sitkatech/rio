export class UserEditAccountsDto {
    AccountIDs: number[];
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
