export class WaterTransferRegisterDto {
    WaterTransferType: number;
    ConfirmingUserID: number;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

