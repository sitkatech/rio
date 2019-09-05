export class SeriesEntry {
    name: string;
    value?: number | string;
}
;
export class MultiSeriesEntry {
    name: string;
    series: SeriesEntry[];
    constructor(name: string, series: {
        name: string;
        value?: number | string;
    }[]) {
        this.name = name;
        this.series = series;
    }
}
