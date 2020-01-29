export class AccountEditUsersDto {
    UserIDs: number[];
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
