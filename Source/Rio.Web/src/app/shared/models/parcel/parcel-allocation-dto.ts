export class ParcelAllocationDto {
    ParcelID: number;
    WaterYear: number;
    ParcelAllocationTypeID: number;
    AcreFeetAllocated: number;
    ParcelAllocationID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}