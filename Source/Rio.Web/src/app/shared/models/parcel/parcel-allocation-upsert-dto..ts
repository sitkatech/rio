export class ParcelAllocationUpsertDto {
    WaterYear: number;
    AcreFeetAllocated: number;
    ParcelAllocationTypeID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}