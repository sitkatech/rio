export const environment = {
  production: true,
  staging: false,
  dev: false,
  apiHostName: 'api-rrbwatertrading.sitkatech.com',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/landowner-dashboard',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: window.location.origin + '/',
    waitForTokenInMsec: 500
  },
  geoserverMapServiceUrl: 'https://rio-geoserver.yachats.sitkatech.com/geoserver/Rio'
};
