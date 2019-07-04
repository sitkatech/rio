export class UserSimpleDto {
    UserID: number;
    FirstName: string;
    LastName: string;
    FullName: string;
    Email: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

