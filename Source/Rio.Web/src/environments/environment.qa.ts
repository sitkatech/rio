export const environment = {
  production: false,
  staging: true,
  dev: false,
  apiHostName: 'rio-api.qa.sycan.sitkatech.com',
  createAccountUrl: "https://qa.keystone.sitkatech.com/Authentication/Register?RedirectUrl=",
  createAccountRedirectUrl: "https://rio.qa.sycan.sitkatech.com/create-user-callback",
  allowTrading: false,

  keystoneSupportBaseUrl: "https://qa.keystone.sitkatech.com/Authentication",

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://qa.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/login-callback',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://qa.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: window.location.origin + '/',
    waitForTokenInMsec: 500
  },
  geoserverMapServiceUrl: 'https://rio-geoserver.qa.sycan.sitkatech.com/geoserver/Rio'
};

