export const environment = {
  production: true,
  staging: false,
  dev: false,
  apiHostName: 'rio-api.yachats.sitkatech.com',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://www.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://www.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: ''
  },
  geoserverMapServiceUrl: 'https://mapserver.projectfirma.com/geoserver/Rio'
};
