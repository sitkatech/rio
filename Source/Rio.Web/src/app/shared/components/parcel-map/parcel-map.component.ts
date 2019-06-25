import {AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges} from '@angular/core';
import {Feature, FeatureCollection, Polygon} from "geojson";
import {environment} from "src/environments/environment";
import {WfsService} from "../../services/wfs.service";
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
export class ParcelMapComponent implements OnInit, AfterViewInit, OnChanges {
    @Input()
    public mapId: string = '';

    @Input()
    public feature?: any;

    @Input()
    public defaultExtent?: FeatureCollection;

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

    constructor() {
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
            "Parcels": tileLayer.wms(`${environment.geoserverMapServiceUrl}/wms?`, {
                layers: "Rio:AllParcels",
                transparent: true,
                format: "image/png",
                tiled: true,
            } as WMSOptions),
        }, this.overlayLayers);
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
        this.map = map(this.mapId, mapOptions);

        this.map.on('load', (event: LeafletEvent)=> {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
        });

        this.map.fitBounds([[35.26, -119.5], [35.47, -119.1]], this.defaultFitBoundsOptions);
        this.map.setMaxBounds([[35.26, -119.5], [35.47, -119.1]]);

        this.setControl();
        this.ngOnChanges(null);

        if (!this.disableDefaultClick) {
//             const wfsService = this.wfsService;
//             const self = this;
//             this.map.on("click", (event: LeafletMouseEvent): void => {
//                 wfsService.getparcelByCoordinate(event.latlng.lng, event.latlng.lat)
//                     .subscribe((parcelFeatureCollection: FeatureCollection) => {
//                         parcelFeatureCollection.features
//                             .forEach((feature: Feature) => {
//                                 // Flip the coordinates
//                                 switch (feature.geometry.type) {
//                                     case "Polygon":
//                                         const polygon: Polygon = feature.geometry as Polygon;
//                                         polygon.coordinates = polygon.coordinates
//                                             .map(coordinate => coordinate.map(point => [point[1], point[0]]));
//                                         break;
//                                 }
//                                 new Popup({
//                                     minWidth: 250,
//                                 })
//                                     .setLatLng(event.latlng)
//                                     .setContent(`
// <dl class="row mb-0">
//     <dt class="col-5 text-right">Trust Area</dt>
//     <dd class="col-7">
//         <a href="/#/trust-areas/${feature.properties.parcelId}" target="_blank">
//             ${feature.properties.strid}
//             <span class="fas fa-external-link-alt"></span>
//         </a>
//     </dd>

//     <dt class="col-5 text-right">Trust</dt>
//     <dd class="col-7">
//         <a href="/#/trusts/${feature.properties.trustId}" target="_blank">
//             ${feature.properties.trustName}
//             <span class="fas fa-external-link-alt"></span>
//         </a>
//     </dd>
// </dl>
// `
//                                     )
//                                     .openOn(self.map);
//                             });
//                    });
//            });
        }
    }

    // using ngOnChanges to make sure that we update the map when we have this.feature
    public ngOnChanges(changes: SimpleChanges): void {
        if (this.defaultExtent && this.map) {
            const defaultExtentLayer = new GeoJSON(this.defaultExtent, {
                style: {
                    "color": "#800080",
                    "weight": 1,
                    "opacity": 0.5,
                    "fillColor": "#800080",
                    "dashArray": "10 5",
                    "fillOpacity": 0.08
                },
                coordsToLatLng: (coords) => {
                    return new LatLng(coords[0], coords[1], coords[2]);
                }
            });
            defaultExtentLayer.addTo(this.map).bringToBack();
            if (this.zoomMapToDefaultExtent) {
                this.map.fitBounds(defaultExtentLayer.getBounds());
            }
        }

        if (!(changes && changes.feature) || !this.map) {
            return;
        }

        if (this.featureLayer) {
            this.map.removeLayer(this.featureLayer);
        }

        const geoJsonOptions: GeoJSONOptions = {
            style: {
                "color": "#205c90",
                "weight": 1,
                "opacity": 1,
                "fillColor": "#205c90",
                "fillOpacity": 0.65
            },
            minZoom: 7,
            maxZoom: 12,
            onEachFeature: this.onEachFeatureCallback,
            // leaflet is overzealously flipping our coordinates because of cartographical traditions
            // setting this property overwrites that behavior so our shapes don't look like they're down in antarctica
            coordsToLatLng: function (coords) {
                return new LatLng(coords[0], coords[1], coords[2])
            }
        } as GeoJSONOptions;
        this.featureLayer = new GeoJSON(this.feature, geoJsonOptions);
        this.featureLayer.addTo(this.map);

        // only resize the map if the feature is not null
        if (this.feature && (this.feature.geometry || this.feature.features.length) && !this.defaultExtent) {
            this.map.fitBounds(this.featureLayer.getBounds());
        }
    }

    public setControl(): void {
        this.control = new Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        if (this.displayparcelsLayerOnLoad) {
            this.overlayLayers["Parcels"].addTo(this.map);
        }
        this.afterSetControl.emit(this.control);
    }
}
