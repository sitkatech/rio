export class LandownerUsageReportDto {
    AccountID: number;
    AccountName: string;
    AccountNumber: string;
    AcresManaged: number;
    Allocation: number;
    Purchased: number;
    Sold: number;
    TotalSupply: number;
    UsageToDate: number;
    CurrentAvailable: number;
    NumberOfPostings: number;
    NumberOfTrades: number;
    MostRecentTradeNumber: string;
    Allocations: {[key : number]: number};

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

