import { TransactionTypeDto } from "../transaction-type-dto";
import { WaterTypeDto } from "../water-type-dto";

export class ParcelLedgerDto {
    ParcelID: number;
    TransactionDate: Date;
    EffectiveDate: Date;
    TransactionType: TransactionTypeDto;
    TransactionAmount: number;
    TransactionDescription: string;
    ParcelLedgerID: number;
    WaterYear: number;
    WaterMonth: number;
    WaterType: WaterTypeDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
