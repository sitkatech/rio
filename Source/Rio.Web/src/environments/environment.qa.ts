export const environment = {
  production: false,
  staging: true,
  dev: false,
  apiHostName: 'rio-api.qa.sycan.sitkatech.com',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://qa.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/landowner-dashboard',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://qa.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: window.location.origin + '/',
    waitForTokenInMsec: 500
  },
  geoserverMapServiceUrl: 'https://rio-geoserver.qa.sycan.sitkatech.com/geoserver/Rio'
};

