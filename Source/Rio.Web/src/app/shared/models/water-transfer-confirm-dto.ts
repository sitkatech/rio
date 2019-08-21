export class WaterTransferConfirmDto {
    WaterTransferType: number;
    ConfirmingUserID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

