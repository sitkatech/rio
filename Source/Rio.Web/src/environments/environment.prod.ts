export const environment = {
  production: true,
  staging: false,
  dev: false,
  apiHostName: 'api.rio.org:8889',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://www.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://www.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: ''
  }
};
