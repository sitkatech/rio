import { Injectable } from '@angular/core';
import { ApplicationInsights, IDependencyTelemetry, ITelemetryItem } from '@microsoft/applicationinsights-web';
import { environment } from 'src/environments/environment';
const { name, version } = require('../../../../package.json');

@Injectable({
  providedIn: 'root'
})
export class AppInsightsService {

  constructor() { }

  initAppInsights() {
    const appInsights = new ApplicationInsights({
      config: {
        appId: name + '@' + version,
        enableAutoRouteTracking: true, 
        disableFetchTracking: false,
        enableCorsCorrelation: true,
        enableRequestHeaderTracking: true,
        enableResponseHeaderTracking: true,
        correlationHeaderExcludedDomains: ['keystone.sitkatech.com', 'qa.keystone.sitkatech.com', new URL(environment.geoserverMapServiceUrl).hostname],
        instrumentationKey: environment.appInsightsInstrumentationKey,
        maxAjaxCallsPerView: -1,
      }
    });
  
    appInsights.loadAppInsights();
  
    appInsights.addTelemetryInitializer((item: ITelemetryItem) => {
      if (
        item &&
        item.baseData &&
        [0, 401].indexOf((item.baseData as IDependencyTelemetry).responseCode) >= 0
      ) {
        return false;
      }
    }); 
    appInsights.addTelemetryInitializer((envelope) => {
      envelope.tags["ai.cloud.role"] = environment.keystoneAuthConfiguration.clientId + ".Web";
    });
  
    (window as any).appInsights = appInsights;
  }
}
