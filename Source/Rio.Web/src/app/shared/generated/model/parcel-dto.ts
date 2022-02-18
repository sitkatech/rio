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
import { AccountDto } from '././account-dto';
import { ParcelStatusDto } from '././parcel-status-dto';

export class ParcelDto { 
    ParcelID?: number;
    ParcelNumber?: string;
    ParcelAreaInSquareFeet?: number;
    ParcelAreaInAcres?: number;
    ParcelStatus?: ParcelStatusDto;
    InactivateDate?: string;
    LandOwner?: AccountDto;
    TagsAsCommaSeparatedString?: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
