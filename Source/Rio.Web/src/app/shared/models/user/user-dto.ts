import { RoleDto } from '../role/role-dto';

export class UserDto {
    UserID: number;
    UserGuid: string;

    FirstName: string;
    LastName: string;
    FullName: string;

    Email: string;
    Phone: string;
    LoginName: string;

    Role: RoleDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export class ParcelChangeOwnerDto {
    ParcelID: number;
    UserID: number;
    OwnerName: number;

    constructor(obj?:any){
        Object.assign(this,obj)
    }
}