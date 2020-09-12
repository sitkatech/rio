export class ParcelAllocationTypeDto {
    ParcelAllocationTypeID: number;
    ParcelAllocationtypeName: string;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}
