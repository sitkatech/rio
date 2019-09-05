import { MultiSeriesEntry, SeriesEntry } from './series-entry';

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

export class WaterAllocationOverviewDto {
    Current: {
        Year: number,
        CumulativeWaterUsage: SeriesEntry[]
    }[];
    Historic: SeriesEntry[]
}
