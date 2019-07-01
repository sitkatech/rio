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

    private getparcelIDsIntersectingUnsubscribe: Subject<void> = new Subject<void>();

    constructor(
        private http: HttpClient,
    ) {
    }

    public getParcelByCoordinate(longitude: number, latitude: number): Observable<FeatureCollection> {
        const url: string = `${environment.geoserverMapServiceUrl}/wms`;
        return this.http.get<FeatureCollection>(url, {
            params: {
                service: 'WFS',
                version: '2.0',
                request: 'GetFeature',
                outputFormat: 'application/json',
                SrsName: 'EPSG:4326',
                typeName: 'Rio:AllParcels',
                cql_filter: `intersects(ParcelGeometry, POINT(${latitude} ${longitude}))`
            }
        });
    }

    public getParcelIdsIntersecting(lon1: number, lon2: number, lat1: number, lat2: number): Observable<number[]> {
        this.getparcelIDsIntersectingUnsubscribe.next();

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
                cql_filter: `bbox(ParcelGeometry,${lat1},${lon1},${lat2},${lon2})`,
            },
        })
            .pipe(
                takeUntil(this.getparcelIDsIntersectingUnsubscribe),
                map((rawData: string) => {
                    // Parse XML to retrieve nodes
                    const parcelIDNodes: HTMLCollection = new DOMParser()
                        .parseFromString(rawData, "text/xml")
                        .getElementsByTagName("heartwood:parcelId");

                    const parcelIDs: number[] = [];
                    for (let i = 0; i < parcelIDNodes.length; i++) {
                        parcelIDs.push(parseInt(parcelIDNodes[i].innerHTML))
                    }
                    return parcelIDs;
                })
            );

    }
}
