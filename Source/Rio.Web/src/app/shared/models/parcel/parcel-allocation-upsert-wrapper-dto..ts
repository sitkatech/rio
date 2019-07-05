import { ParcelAllocationUpsertDto } from './parcel-allocation-upsert-dto.';

export class ParcelAllocationUpsertWrapperDto {
    ParcelAllocations: Array<ParcelAllocationUpsertDto>;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}