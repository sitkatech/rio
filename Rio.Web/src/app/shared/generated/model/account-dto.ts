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
import { AccountStatusDto } from '././account-status-dto';
import { UserSimpleDto } from '././user-simple-dto';

export class AccountDto { 
    Users?: Array<UserSimpleDto>;
    AccountDisplayName?: string;
    ShortAccountDisplayName?: string;
    NumberOfParcels?: number;
    NumberOfUsers?: number;
    AccountID?: number;
    AccountNumber?: number;
    AccountName?: string;
    AccountStatus?: AccountStatusDto;
    Notes?: string;
    UpdateDate?: string;
    AccountVerificationKey?: string;
    AccountVerificationKeyLastUseDate?: string;
    CreateDate?: string;
    InactivateDate?: string;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
