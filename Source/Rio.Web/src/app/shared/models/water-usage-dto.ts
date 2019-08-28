export class WaterUsageDto {
    Year: number;
    WaterUsage: MonthlyWaterUsageDto[]
}

export class MonthlyWaterUsageDto{
    Month: string;

    WaterUsageByParcel: {
        ParcelNumber: string;
        WaterUsageInAcreFeet?: number | string;
    }[];

    // boilerplate reshaper to ngx-charts format
    public toMultiSeriesEntry() : MultiSeriesEntry {
        return new MultiSeriesEntry(this.Month, this.WaterUsageByParcel.map(x=> {
            let y: SeriesEntry;
            y.name = x.ParcelNumber;
            y.value = x.WaterUsageInAcreFeet
            return y;
        }));
    }
}

type SeriesEntry = {
    name: string;
    value? : number | string;
};

export class MultiSeriesEntry {
    name:string;
    series: SeriesEntry[];
    constructor(name:string, series: {name:string, value? : number | string}[]){
        this.name = name;
        this.series = series;
    }
}
