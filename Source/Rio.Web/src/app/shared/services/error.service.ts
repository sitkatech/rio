import { Injectable, ErrorHandler, Injector } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http'
import { ApiService } from './api/api.service';
import { ApiRouteService } from './api-route/api-route.service';
@Injectable({
  providedIn: 'root'
})
export class ErrorService implements ErrorHandler {
  constructor(private injector: Injector,
    private apiService: ApiService) { }
  handleError(error: any) {
    const router = this.injector.get(Router);
    if (error instanceof HttpErrorResponse) {
      return;
    }

    let errorObj = new ErrorMessage();
    errorObj.Message = error.message;
    errorObj.Stack = error.stack;

    this.apiService.postToApi("error/front-end", errorObj).subscribe(x => {
      console.log("Error successfully logged");
    })
  }
}

export class ErrorMessage {
  Message : string
  Stack : string

  constructor(obj?: any){
    Object.assign(this, obj);
}
}