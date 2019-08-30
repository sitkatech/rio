export class WaterUsageDto {
    Year: number;
    WaterUsage: MonthlyWaterUsageDto[]

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export class MonthlyWaterUsageDto {
    Month: string;

    WaterUsageByParcel: {
        ParcelNumber: string;
        WaterUsageInAcreFeet: number;
    }[];

    constructor(obj?: any) {
        Object.assign(this, obj);
    }

    // boilerplate reshaper to ngx-charts format
    public toMultiSeriesEntry(): MultiSeriesEntry {
        return new MultiSeriesEntry(this.Month, this.WaterUsageByParcel.map(x => {
            let y = new SeriesEntry();
            y.name = x.ParcelNumber;
            y.value = x.WaterUsageInAcreFeet
            return y;
        }));
    }
}

export class SeriesEntry {
    name: string;
    value?: number | string;
};

export class MultiSeriesEntry {
    name: string;
    series: SeriesEntry[];

    constructor(name: string, series: { name: string, value?: number | string }[]) {
        this.name = name;
        this.series = series;
    }
}

export class WaterUsageOverviewDto {
    Current: {
        Year: number,
        CumulativeWaterUsage: SeriesEntry[]
    }[];
    Historic: SeriesEntry[]
}
