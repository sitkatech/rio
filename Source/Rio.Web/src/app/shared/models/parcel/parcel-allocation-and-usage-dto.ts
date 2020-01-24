import { AccountSimpleDto } from '../account/account-simple-dto';

export class ParcelAllocationAndUsageDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelAreaInAcres: number;
    LandOwner: AccountSimpleDto;
    Allocation: number;
    ProjectWater: number;
    Reconciliation: number;
    NativeYield: number;
    StoredWater: number;
    UsageToDate: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}