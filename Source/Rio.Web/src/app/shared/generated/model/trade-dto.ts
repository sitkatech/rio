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
import { TradeStatusDto } from '././trade-status-dto';
import { PostingDto } from '././posting-dto';
import { AccountDto } from '././account-dto';

export class TradeDto { 
    tradeID?: number;
    tradeNumber?: string;
    createAccount?: AccountDto;
    tradeStatus?: TradeStatusDto;
    posting?: PostingDto;
    constructor(obj?: any) {
        Object.assign(this, obj);
    }
}
