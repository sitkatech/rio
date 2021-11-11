import { StringLiteralLike } from "typescript";
import { TransactionTypeDto } from "../transaction-type-dto";
import { WaterTypeDto } from "../water-type-dto";

export class ParcelLedgerDto {
    ParcelID: number;
    ParcelNumber: string;
    TransactionDate: Date;
    EffectiveDate: Date;
    TransactionType: TransactionTypeDto;
    TransactionAmount: number;
    TransactionDescription: string;
    ParcelLedgerID: number;
    WaterYear: number;
    WaterMonth: number;
    WaterType: WaterTypeDto;
    UserComment: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
