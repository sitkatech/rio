export class TradeActivityByMonthDto {
    GroupingDate: string;
    TradeVolume: number;
    NumberOfTrades: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

