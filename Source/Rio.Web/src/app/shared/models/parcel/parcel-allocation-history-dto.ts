export class ParcelAllocationHistoryDto {
    Date: Date;
    WaterYear: string;
    Allocation: string;
    Value: string;
    User: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}