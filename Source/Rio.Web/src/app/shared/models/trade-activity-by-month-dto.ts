export class TradeActivityByMonthDto {
    GroupingDate: string;
    TradeVolume: number;
    NumberOfTrades: number;
    MaximumPrice: number;
    MinimumPrice: number;
    AveragePrice: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

