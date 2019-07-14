export class TradeStatusDto {
    TradeStatusID: number;
    TradeStatusName: string;
    TradeStatusDisplayName: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}