export class TransactionTypeDto {
    TransactionTypeID: number;
    TransactionTypeName: string;
    IsAllocation: boolean;
    SortOrder: number;

    constructor(obj?: any){
        Object.assign(this, obj);
    }
}