export const environment = {
  production: false,
  staging: true,
  dev: false,
  apiHostName: 'rio-api.qa.sycan.sitkatech.com',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://qa.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://qa.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: ''
  },
  geoserverMapServiceUrl: 'http://rio-geoserver.qa.sycan.sitkatech.com/geoserver/Rio'
};

