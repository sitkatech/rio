export class WaterTypeDto {
    WaterTypeID: number;
    WaterTypeName: string;
    IsAppliedProportionally: WaterTypeApplicationTypeEnum;
    WaterTypeDefinition: string;
    SortOrder: number;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}

export enum WaterTypeApplicationTypeEnum{
    Spreadsheet = 0,
    Proportional = 1,
    Api = 2
}