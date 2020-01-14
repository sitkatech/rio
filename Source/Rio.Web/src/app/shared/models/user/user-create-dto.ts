export class UserCreateDto {
    FirstName: string;
    LastName: string;
    Email: string;
    OrganizationName: string;
    PhoneNumber: string;
    RoleID: number;
    LoginName: string;
    UserGuid: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
