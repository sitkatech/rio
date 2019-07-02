export const environment = {
  production: true,
  staging: false,
  dev: false,
  apiHostName: 'api-rrbwatertrading.sitkatech.com',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: ''
  },
  geoserverMapServiceUrl: 'https://rio-geoserver.yachats.sitkatech.com/geoserver/Rio'
};
