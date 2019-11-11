import { UserSimpleDto } from '../user/user-simple-dto';

export class ParcelAllocationAndUsageDto {
    ParcelID: number;
    ParcelNumber: string;
    ParcelAreaInAcres: number;
    LandOwner: UserSimpleDto;
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