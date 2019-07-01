import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Feature, FeatureCollection, Polygon } from "geojson";
import { environment } from "src/environments/environment";
import { WfsService } from "../../services/wfs.service";
import {
    Control, FitBoundsOptions,
    GeoJSON,
    GeoJSONOptions,
    LatLng, LeafletEvent,
    LeafletMouseEvent,
    map,
    Map,
    MapOptions,
    Popup,
    tileLayer,
    WMSOptions
} from 'leaflet';

@Component({
    selector: 'parcel-map',
    templateUrl: './parcel-map.component.html',
    styleUrls: ['./parcel-map.component.scss']
})
export class ParcelMapComponent implements OnInit, AfterViewInit {
    @Input()
    public mapID: string = '';

    @Input()
    public selectedParcelIDs: Array<number> = [];

    @Input()
    public onEachFeatureCallback?: (feature, layer) => void;

    @Input()
    public zoomMapToDefaultExtent: boolean = true;

    @Input()
    public disableDefaultClick: boolean = false;

    @Input()
    public displayparcelsLayerOnLoad: boolean = true;

    @Input()
    public mapHeight: string = '300px';

    @Input()
    public defaultFitBoundsOptions?: FitBoundsOptions = null;

    @Output()
    public afterSetControl: EventEmitter<Control.Layers> = new EventEmitter();

    @Output()
    public afterLoadMap: EventEmitter<LeafletEvent> = new EventEmitter();

    @Output()
    public onMapMoveEnd: EventEmitter<LeafletEvent> = new EventEmitter();

    public map: Map;
    public featureLayer: any;
    public control: Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};

    constructor(
        private wfsService: WfsService,
    ) {
    }

    public ngOnInit(): void {
        this.tileLayers = Object.assign({}, {
            "Aerial": tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Aerial',
            }),
            "Street": tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Aerial',
            }),
            "Terrain": tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Terrain',
            }),
        }, this.tileLayers);

        this.overlayLayers = Object.assign({
            "Parcels": tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", {
                layers: "Rio:AllParcels",
                transparent: true,
                format: "image/png",
                tiled: true,
            } as WMSOptions)
        }, this.overlayLayers);

        if (this.selectedParcelIDs.length > 0) {
            this.overlayLayers = Object.assign({
                "My Parcels": tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", {
                    layers: "Rio:AllParcels",
                    transparent: true,
                    format: "image/png",
                    tiled: true,
                    styles: "parcel_yellow",
                    cql_filter: "ParcelID in (" + this.selectedParcelIDs.join(',') + ")"
                } as WMSOptions)
            }, this.overlayLayers);
        }

    }

    public ngAfterViewInit(): void {

        const mapOptions: MapOptions = {
            // center: [46.8797, -110],
            // zoom: 6,
            minZoom: 6,
            maxZoom: 17,
            layers: [
                this.tileLayers["Aerial"],
            ],
        } as MapOptions;
        this.map = map(this.mapID, mapOptions);

        this.map.on('load', (event: LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
        });

        this.map.fitBounds([[35.34, -119.43], [35.35, -119.15]], this.defaultFitBoundsOptions);
        //this.map.setMaxBounds([[35.26, -119.5], [35.47, -119.1]]);

        this.setControl();

        if (!this.disableDefaultClick) {
            const wfsService = this.wfsService;
            const self = this;
            this.map.on("click", (event: LeafletMouseEvent): void => {
                wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat)
                    .subscribe((parcelFeatureCollection: FeatureCollection) => {
                        parcelFeatureCollection.features
                            .forEach((feature: Feature) => {
                                // Flip the coordinates
                                switch (feature.geometry.type) {
                                    case "Polygon":
                                        const polygon: Polygon = feature.geometry as Polygon;
                                        polygon.coordinates = polygon.coordinates
                                            .map(coordinate => coordinate.map(point => [point[1], point[0]]));
                                        break;
                                }
                                new Popup({
                                    minWidth: 250,
                                })
                                    .setLatLng(event.latlng)
                                    .setContent(`
<dl class="row mb-0">
    <dt class="col-5 text-right">Trust Area</dt>
    <dd class="col-7">
        <a href="/#/trust-areas/${feature.properties.ParcelID}" target="_blank">
            ${feature.properties.ParcelNumber}
            <span class="fas fa-external-link-alt"></span>
        </a>
    </dd>
</dl>
`
                                    )
                                    .openOn(self.map);
                            });
                    });
            });
        }
    }

    public setControl(): void {
        this.control = new Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        if (this.displayparcelsLayerOnLoad) {
            this.overlayLayers["Parcels"].addTo(this.map);
        }
        if(this.selectedParcelIDs.length > 0)
        {
            const myParcelsLayer = this.overlayLayers["My Parcels"];
            myParcelsLayer.addTo(this.map);
        }
        this.afterSetControl.emit(this.control);
    }
}
