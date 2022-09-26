import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserMessageDto } from '../shared/generated/model/user-message-dto';
import { ApiService } from '../shared/services';

@Injectable({
  providedIn: 'root'
})
export class UserMessageService {

  constructor(private apiService: ApiService) { }

  getUserMessageFromUserMessageID(userMessageID: number): Observable<UserMessageDto> {
    let route = `/user-messages/${userMessageID}`;
    return this.apiService.getFromApi(route);
  }

  createNewUserMessage(userMessageSimpleDto: any): Observable<UserMessageDto> {
    let route = `/user-messages/new`;
    return this.apiService.postToApi(route, userMessageSimpleDto);
  }
}
