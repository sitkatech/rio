
export class WaterYearDto {
    WaterYearID: number;
    Year: number;
    ParcelLayerUpdateDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

