// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.

export const environment = {
  production: false,
  staging: false,
  dev: true,
  apiHostName: 'api-dev.rio.org:8889',

  keystoneAuthConfiguration: {
    clientId: 'Rio',
    issuer: 'https://qa.keystone.sitkatech.com/core',
    redirectUri: window.location.origin + '/',
    scope: 'openid all_claims keystone',
    sessionChecksEnabled: true,
    logoutUrl: 'https://qa.keystone.sitkatech.com/core/connect/endsession',
    postLogoutRedirectUri: ''
  },
  geoserverMapServiceUrl: 'https://qa-mapserver.projectfirma.com/geoserver/Rio'
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
