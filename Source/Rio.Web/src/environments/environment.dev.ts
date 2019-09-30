export const environment = {
  production: false,
  staging: false,
  dev: true,
  apiHostName: 'api-dev.rio.org:8889',

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
  geoserverMapServiceUrl: 'http://localhost:8777/geoserver/Rio'
};
