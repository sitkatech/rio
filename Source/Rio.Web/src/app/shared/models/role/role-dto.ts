export class RoleDto {
    RoleID: number;
    RoleName: string;
    RoleDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}