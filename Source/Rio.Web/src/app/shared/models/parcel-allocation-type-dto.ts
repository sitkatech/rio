export class ParcelAllocationTypeDto {
    ParcelAllocationTypeID: number;
    ParcelAllocationTypeName: string;
    IsAppliedProportionally: ParcelAllocationTypeApplicationTypeEnum;
    ParcelAllocationTypeDefinition: string;
    SortOrder: number;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}

export enum ParcelAllocationTypeApplicationTypeEnum{
    Spreadsheet = 0,
    Proportional = 1,
    Api = 2
}