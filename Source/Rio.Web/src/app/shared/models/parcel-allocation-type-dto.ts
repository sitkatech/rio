export class ParcelAllocationTypeDto {
    ParcelAllocationTypeID: number;
    ParcelAllocationtypeName: string;
    IsAppliedProportionally: boolean;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}
