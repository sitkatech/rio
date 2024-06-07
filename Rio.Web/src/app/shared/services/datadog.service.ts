import { Injectable } from '@angular/core';
import { LogsEvent, datadogLogs } from '@datadog/browser-logs';
import { NetworkLogsEventDomainContext } from '@datadog/browser-logs/cjs/domainContext.types';
import { environment } from 'src/environments/environment';
import { AuthenticationService } from 'src/app/services/authentication.service';
import { Observable, forkJoin } from 'rxjs';
import { UserDto } from '../generated/model/user-dto';
import { SystemInfoService } from 'src/app/services/system-info.service';

@Injectable({
  providedIn: 'root'
})
export class DatadogService {

  private currentUser$: Observable<UserDto>;
  constructor(private systemInfoService: SystemInfoService, private authenticationService: AuthenticationService) { }
  
  init() {
    //NOTE: Logs are only sent if there is a logged in user.
    this.currentUser$ = this.authenticationService.getCurrentUser();



    forkJoin([this.systemInfoService.getSystemInfo(), this.currentUser$]).subscribe(([systemInfo, currentUser]) => {
      const env = environment.production ? 'prod' : environment.staging? 'qa' : 'dev';

      datadogLogs.init({
        clientToken: environment.datadogClientToken,
        site: 'datadoghq.com',
        forwardErrorsToLogs: true,
        version: systemInfo.Version,
        sessionSampleRate: 100,
        service: 'rio-angular', // NOTE: Update with application name, this is used to group logs in Datadog
        env,

        beforeSend: (log: LogsEvent, context: NetworkLogsEventDomainContext) => {
          log.team = 'jackalope'; // NOTE: Update with team name
          log.userGuid = currentUser.UserGuid;
          log.environment = env;

          return true;
        },
      });
    });

  }
}