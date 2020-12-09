declare var window: any;

export class DynamicEnvironment {
    private _production: boolean;

    constructor(_production: boolean){
        this._production = _production
    }

    public get production() {
        if (window.config) {
            return window.config.production;
        } else return this._production;
    }

    public get staging() {
        return window.config.staging;
    }

    public get dev() {
        return window.config.dev;
    }

    public get apiHostName() {
        return window.config.apiHostName;
    }

    public get createAccountUrl() {
        return window.config.createAccountUrl;
    }

    public get createAccountRedirectUrl() {
        return window.config.createAccountRedirectUrl;
    }

    public get allowTrading() {
        return window.config.allowTrading;
    }

    public get keystoneSupportBaseUrl() {
        return window.config.keystoneSupportBaseUrl;
    }

    public get geoserverMapServiceUrl() {
        return window.config.geoserverMapServiceUrl;
    }

    public get keystoneAuthConfiguration() {
        return window.config.keystoneAuthConfiguration;
    }

    public get platformLongName(){
        return window.config.platformLongName;
    }

    public get platformShortName(){
        return window.config.platformShortName;
    }

    public get leadOrganizationLongName(){
        return window.config.leadOrganizationLongName;
    }

    public get leadOrganizationShortName(){
        return window.config.leadOrganizationShortName;
    }

    public get leadOrganizationHomeUrl(){
        return window.config.leadOrganizationHomeUrl;
    }

    public get faviconFilename(){
        return window.config.faviconFilename;
    }

    public get leadOrganizationLogoFilename(){
        return window.config.leadOrganizationLogoFilename;
    }

    public get enabledGETIntegration() {
        return window.config.enabledGETIntegration;
    }

    public get contactInfoPhone() {
        return window.config.contactInfoPhone;
    }

    public get contactInfoEmail() {
        return window.config.contactInfoEmail;
    }

    public get contactInfoMailingAddress() {
        return window.config.contactInfoMailingAddress;
    }

    public get contactInfoPhysicalAddress() {
        return window.config.contactInfoPhysicalAddress;
    }

    public get navThemeColor() {
        return window.config.navThemeColor;
    }

    public get applicationType() {
        return window.config.applicationType;
    }

    public get applicationInternalName() {
        return window.config.applicationInternalName;
    }

    public get allowOpenETSync() {
        return window.config.allowOpenETSync;
    }

    public get appInsightsInstrumentationKey() {
        return window.config.appInsightsInstrumentationKey;
    }
}
