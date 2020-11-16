import { ParcelSimpleDto } from '../parcel/parcel-simple-dto';
import { AccountDto } from './account-dto';
export class AccountIncludeParcelsDto {
    Account: AccountDto;
    Parcels: Array<ParcelSimpleDto>;
    
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}