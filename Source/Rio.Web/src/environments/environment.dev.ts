export const environment = {
  production: false,
  staging: false,
  dev: true,
  apiHostName: 'api-dev.rio.org:8889',
  createAccountUrl: "https://qa.keystone.sitkatech.com/Authentication/Register?RedirectUrl=",
  createAccountRedirectUrl: "http://dev.rio.org:8887/create-user-callback",
  allowTrading: false,

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
  geoserverMapServiceUrl: 'http://localhost:8780/geoserver/Rio'
};
