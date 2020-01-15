export const environment = {
  production: true,
  staging: false,
  dev: false,
  apiHostName: 'api-rrbwatertrading.sitkatech.com',
  createAccountUrl: "https://keystone.sitkatech.com/Authentication/Register?RedirectUrl=",
  createAccountRedirectUrl: "https://waterbudget.rrbwsd.com/",

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/login-callback',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: window.location.origin + '/',
    waitForTokenInMsec: 500
  },
  geoserverMapServiceUrl: 'https://rio-geoserver.yachats.sitkatech.com/geoserver/Rio'
};
