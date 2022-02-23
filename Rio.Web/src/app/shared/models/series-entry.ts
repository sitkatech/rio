export class SeriesEntry {
    name: string;
    value?: any;
    isEmpty?: boolean;

    constructor(name: string, value?: any, isEmpty?: boolean) {
        this.name = name;
        this.value = value;
        this.isEmpty = isEmpty;
    }
}
;
export class MultiSeriesEntry {
    name: string;
    series: SeriesEntry[];
    constructor(name: string, series: {
        name: string;
        value?: any;
    }[]) {
        this.name = name;
        this.series = series;
    }
}
