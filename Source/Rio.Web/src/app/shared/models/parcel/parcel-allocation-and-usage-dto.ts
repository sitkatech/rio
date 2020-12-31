import { AccountSimpleDto } from '../account/account-simple-dto';
import { ParcelStatusDto } from './parcel-status-dto';

export class ParcelAllocationAndUsageDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelStatus: ParcelStatusDto;
    ParcelAreaInAcres: number;
    LandOwner: AccountSimpleDto;
    Allocation: number;
    ProjectWater: number;
    Reconciliation: number;
    NativeYield: number;
    StoredWater: number;
    UsageToDate: number;
    Allocations: {[key : number]: number};

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

