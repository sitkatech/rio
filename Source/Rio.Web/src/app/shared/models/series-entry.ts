export class SeriesEntry {
    name: string;
    value?: number;
    constructor(name: string, value? : number) {
        this.name = name;
        this.value = value;
    }
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
