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

export class TransactionHistoryDto { 
    TransactionDate?: string;
    EffectiveDate?: string;
    CreateUserFullName?: string;
    WaterTypeName?: string;
    TransactionVolume?: number;
    TransactionDepth?: number;
    UploadedFileName?: string;
    AffectedParcelsCount?: number;
    AffectedAcresCount?: number;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
