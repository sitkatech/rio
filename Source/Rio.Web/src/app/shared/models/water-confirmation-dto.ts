export class WaterConfirmationDto {
    ConfirmationDate: Date;
    ConfirmationType: string;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}

