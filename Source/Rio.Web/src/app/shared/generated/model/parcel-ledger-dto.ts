/**
 * Rio.API
 * No description provided (generated by Openapi Generator https://github.com/openapitools/openapi-generator)
 *
 * OpenAPI spec version: 1.0
 * 
 *
 * NOTE: This class is auto generated by the swagger code generator program.
 * https://github.com/swagger-api/swagger-codegen.git
 * Do not edit the class manually.
 */
import { TransactionTypeDto } from '././transaction-type-dto';
import { UserDto } from '././user-dto';
import { ParcelDto } from '././parcel-dto';
import { WaterTypeDto } from '././water-type-dto';

export class ParcelLedgerDto { 
    ParcelLedgerID?: number;
    Parcel?: ParcelDto;
    TransactionDate?: string;
    EffectiveDate?: string;
    TransactionType?: TransactionTypeDto;
    TransactionAmount?: number;
    WaterType?: WaterTypeDto;
    TransactionDescription?: string;
    User?: UserDto;
    UserComment?: string;
    readonly WaterYear?: number;
    readonly WaterMonth?: number;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
