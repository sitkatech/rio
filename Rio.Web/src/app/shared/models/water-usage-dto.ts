import { MultiSeriesEntry, SeriesEntry } from './series-entry';

export class WaterUsageDto {
    Year: number;
    WaterUsage: MultiSeriesEntry[]

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

export class WaterSupplyOverviewDto {
    Current: {
        Year: number,
        CumulativeWaterUsage: SeriesEntry[]
    }[];
    Historic: SeriesEntry[]
}
