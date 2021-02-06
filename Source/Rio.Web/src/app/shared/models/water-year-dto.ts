
export class WaterYearDto {
    WaterYearID: number;
    Year: number;
    FinalizeDate: Date;
    ParcelLayerUpdateDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}


