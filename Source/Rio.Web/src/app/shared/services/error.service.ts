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
    
    let errorString = ""

    if (typeof error === 'string') {
      errorString = error;
    }
    
    if (typeof error === 'object' && error.hasOwnProperty("stack")) {
      errorString = error.stack;
    }

    this.apiService.postToApi("error/front-end", {'Error':errorString}).subscribe(x => {
      console.log("Error reported to support staff");
    })
  }
}