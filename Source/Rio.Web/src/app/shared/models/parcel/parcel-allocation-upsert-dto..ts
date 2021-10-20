export class ParcelAllocationUpsertDto {
    WaterYear: number;
    AcreFeetAllocated: number;
    WaterTypeID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}