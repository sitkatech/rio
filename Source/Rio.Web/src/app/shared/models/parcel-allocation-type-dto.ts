export class ParcelAllocationTypeDto {
    ParcelAllocationTypeID: number;
    ParcelAllocationTypeName: string;
    IsAppliedProportionally: boolean;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}
