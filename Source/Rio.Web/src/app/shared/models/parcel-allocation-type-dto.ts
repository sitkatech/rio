export class ParcelAllocationTypeDto {
    ParcelAllocationTypeID: number;
    ParcelAllocationTypeName: string;
    IsAppliedProportionally: boolean;
    ParcelAllocationTypeDefinition: string;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}
