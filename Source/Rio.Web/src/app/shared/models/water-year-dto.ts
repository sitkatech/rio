
export class WaterYearDto {
    WaterYearID: number;
    Year: number;
    FinalizeDate: Date;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
