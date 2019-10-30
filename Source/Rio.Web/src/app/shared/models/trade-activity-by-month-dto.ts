export class TradeActivityByMonthDto {
    MonthYear: string;
    TradeVolume: number;
    NumberOfTrades: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

