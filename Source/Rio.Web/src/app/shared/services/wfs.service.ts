import {Injectable} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {FeatureCollection} from "geojson";
import {Observable, Subject} from "rxjs";
import {map, takeUntil} from "rxjs/operators";
import {environment} from "src/environments/environment";

@Injectable({
    providedIn: 'root'
})
export class WfsService {

    private getparcelIdsIntersectingUnsubscribe: Subject<void> = new Subject<void>();

    constructor(
        private http: HttpClient,
    ) {
    }

    public getparcelIdsIntersecting(lon1: number, lon2: number, lat1: number, lat2: number): Observable<number[]> {
        this.getparcelIdsIntersectingUnsubscribe.next();

        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http.get(url, {
            responseType: "text",
            params: {
                service: "wfs",
                version: "2.0",
                request: "GetPropertyValue",
                SrsName: "EPSG:4326",
                typeName: "Rio:AllParcels",
                valueReference: "ParcelID",
                cql_filter: `bbox(parcelGeometry,${lat1},${lon1},${lat2},${lon2})`,
            },
        })
            .pipe(
                takeUntil(this.getparcelIdsIntersectingUnsubscribe),
                map((rawData: string) => {
                    // Parse XML to retrieve nodes
                    const parcelIdNodes: HTMLCollection = new DOMParser()
                        .parseFromString(rawData, "text/xml")
                        .getElementsByTagName("heartwood:parcelId");

                    const parcelIds: number[] = [];
                    for (let i = 0; i < parcelIdNodes.length; i++) {
                        parcelIds.push(parseInt(parcelIdNodes[i].innerHTML))
                    }
                    return parcelIds;
                })
            );

    }
}
