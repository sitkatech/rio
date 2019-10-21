export class SeriesEntry {
    name: string;
    value?: number;
}
;
export class MultiSeriesEntry {
    name: string;
    series: SeriesEntry[];
    constructor(name: string, series: {
        name: string;
        value?: number;
    }[]) {
        this.name = name;
        this.series = series;
    }
}
