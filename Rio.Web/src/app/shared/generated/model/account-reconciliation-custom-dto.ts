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
import { AccountSimpleDto } from '././account-simple-dto';
import { ParcelSimpleDto } from '././parcel-simple-dto';

export class AccountReconciliationCustomDto { 
    Parcel?: ParcelSimpleDto;
    LastKnownOwner?: AccountSimpleDto;
    AccountsClaimingOwnership?: Array<AccountSimpleDto>;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}