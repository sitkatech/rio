import { AccountSimpleDto } from "../account/account-simple-dto";
import { WaterYearDto } from "../water-year-dto";

export class ParcelOwnershipDto {
    Account : AccountSimpleDto;
    WaterYear : WaterYearDto;

    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}