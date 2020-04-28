import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ReconciliationAllocationService {

  constructor(private httpClient: HttpClient) { }

  uploadFile(file: any): Observable<any> {
    const apiHostName = environment.apiHostName
    const route = `https://${apiHostName}/reconciliationAllocation/upload`;
    var result = this.httpClient.post<any>(
      route,
      file, // Send the File Blob as the POST body.
      {
        // NOTE: Because we are posting a Blob (File is a specialized Blob
        // object) as the POST body, we have to include the Content-Type
        // header. If we don't, the server will try to parse the body as
        // plain text.
        headers: {
          "Content-Type": file.type
        },
        params: {
          clientFilename: file.name,
          mimeType: file.type
        }
      }
    );

    return result;
  }
}
