export class ParcelAllocationUpsertDto {
    WaterYear: number;
    AcreFeetAllocated: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}