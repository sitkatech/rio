import { AccountSimpleDto } from "./account/account-simple-dto";
import { ParcelSimpleDto } from "./parcel/parcel-simple-dto";


export class AccountReconciliationDto {
    Parcel: ParcelSimpleDto;
    LastKnownOwner: AccountSimpleDto;
    AccountsClaimingOwnership: Array<AccountSimpleDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
