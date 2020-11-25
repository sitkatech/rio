import { AfterViewInit, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, ChangeDetectionStrategy, ComponentFactoryResolver, Injector, ApplicationRef, ComponentRef } from '@angular/core';
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
import '../../../../../node_modules/leaflet.fullscreen/Control.FullScreen.js';
import '../../../../../node_modules/leaflet-loading/src/Control.Loading.js';
import { forkJoin } from 'rxjs';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { BoundingBoxDto } from '../../models/bounding-box-dto';
import { ParcelDetailPopupComponent } from '../parcel-detail-popup/parcel-detail-popup.component';
import { CustomCompileService } from '../../services/custom-compile.service';

@Component({
    selector: 'parcel-map',
    templateUrl: './parcel-map.component.html',
    styleUrls: ['./parcel-map.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ParcelMapComponent implements OnInit, AfterViewInit {
    @Input()
    public mapID: string = '';

    @Input()
    public visibleParcelStyle: string = 'parcel';

    @Input()
    public visibleParcelIDs: Array<number> = [];

    @Input()
    public selectedParcelStyle: string = 'parcel_blue';

    @Input()
    public selectedParcelIDs: Array<number> = [];

    @Input()
    public highlightedParcelStyle: string = 'parcel_yellow';

    private _highlightedParcelID: number = null;

    @Input() set highlightedParcelID(value: number) {
        if (this.highlightedParcelID != value) {
            this._highlightedParcelID = value;
            this.highlightParcel();
        }
     }
     
    get highlightedParcelID(): number {     
        return this._highlightedParcelID;    
    }

    @Output()
    public highlightedParcelIDChange: EventEmitter<number> = new EventEmitter<number>();

    @Input()
    public onEachFeatureCallback?: (feature, layer) => void;

    @Input()
    public zoomMapToDefaultExtent: boolean = true;

    @Input()
    public disableDefaultClick: boolean = false;

    @Input()
    public highlightParcelOnClick: boolean = false;

    @Input()
    public displayparcelsLayerOnLoad: boolean = true;

    @Input()
    public mapHeight: string = '300px';

    @Input()
    public defaultFitBoundsOptions?: FitBoundsOptions = null;

    @Input()
    public selectedParcelLayerName: string = "<img src='./assets/main/images/parcel_blue.png' style='height:16px; margin-bottom:3px'> Selected Parcels";

    @Output()
    public afterSetControl: EventEmitter<Control.Layers> = new EventEmitter();

    @Output()
    public afterLoadMap: EventEmitter<LeafletEvent> = new EventEmitter();

    @Output()
    public onMapMoveEnd: EventEmitter<LeafletEvent> = new EventEmitter();

    public wellLayerName: string = "<img src='./assets/main/images/well.png' style='height:16px; margin-bottom:3px'> Wells";
    public parcelLayerName: string = "<img src='./assets/main/images/parcel.png' style='height:16px; margin-bottom:3px'> Parcels";
    public component: any;

    public map: Map;
    public featureLayer: any;
    public layerControl: Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};
    boundingBox: BoundingBoxDto;
    private selectedParcelLayer: any;
    private highlightedParcelLayer: any;
    private defaultParcelsWMSOptions: WMSOptions;

    constructor(
        private wfsService: WfsService,
        private parcelService: ParcelService,
        private appRef: ApplicationRef,
        private compileService: CustomCompileService
    ) {
    }

    public ngOnInit(): void {
        // Default bounding box
        this.boundingBox = new BoundingBoxDto();
        this.boundingBox.Left = -119.11015104115182;
        this.boundingBox.Bottom = 35.442022035628575;
        this.boundingBox.Right = -119.45272037350193;
        this.boundingBox.Top = 35.27608156273151;

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

        this.defaultParcelsWMSOptions = ({
            layers: "Rio:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true
        } as WMSOptions);

        let parcelsWMSOptions = Object.assign({ styles: this.visibleParcelStyle }, this.defaultParcelsWMSOptions);
        if (this.visibleParcelIDs.length > 0) {
            parcelsWMSOptions.cql_filter = this.createParcelMapFilter(this.visibleParcelIDs);
            this.fitBoundsToSelectedParcels(this.visibleParcelIDs);
        }

        let wellsWMSOptions = ({
            layers: "Rio:Wells",
            transparent: true,
            format: "image/png",
            tiled: true
        } as WMSOptions);

        this.overlayLayers = Object.assign({
            [this.parcelLayerName]: tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", parcelsWMSOptions),
            [this.wellLayerName]: tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wellsWMSOptions)
        }, this.overlayLayers);

        this.compileService.configure(this.appRef);
    }

    public updateSelectedParcelsOverlayLayer(parcelIDs: Array<number>) {
        if (this.selectedParcelLayer) {
            this.layerControl.removeLayer(this.selectedParcelLayer);
            this.map.removeLayer(this.selectedParcelLayer);
        }

        var wmsParameters = Object.assign({ styles: this.selectedParcelStyle, cql_filter: this.createParcelMapFilter(parcelIDs) }, this.defaultParcelsWMSOptions);
        this.selectedParcelLayer = tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsParameters);
        this.layerControl.addOverlay(this.selectedParcelLayer, this.selectedParcelLayerName);

        this.selectedParcelLayer.addTo(this.map).bringToFront();
        if (this.highlightedParcelLayer) {
            this.highlightedParcelLayer.bringToFront();
        }
        this.overlayLayers[this.wellLayerName].bringToFront();
    }

    private fitBoundsToSelectedParcels(parcelIDs: Array<number>) {
        this.parcelService.getBoundingBoxByParcelIDs(parcelIDs).subscribe(boundingBox => {
            this.boundingBox = boundingBox;
            this.map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);
        });
    }

    private createParcelMapFilter(parcelIDs: Array<number>): any {
        return "ParcelID in (" + parcelIDs.join(',') + ")";
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
            fullscreenControl: true
        } as MapOptions;
        this.map = map(this.mapID, mapOptions);

        this.map.on('load', (event: LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
        });
        this.map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);

        this.setControl();

        if (this.selectedParcelIDs.length > 0) {
            this.updateSelectedParcelsOverlayLayer(this.selectedParcelIDs);
            this.fitBoundsToSelectedParcels(this.selectedParcelIDs);
        }

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
                                    .setContent(this.compileService.compile(ParcelDetailPopupComponent, (c) => { c.instance.feature = feature; })
                                    )
                                    .openOn(self.map);
                            });
                    });
            });
        }

        if (this.highlightParcelOnClick) {
            const wfsService = this.wfsService;
            this.map.on("click", (event: LeafletMouseEvent): void => {
                this.map.fireEvent("dataloading");
                wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat)
                    .subscribe((parcelFeatureCollection: FeatureCollection) => {
                        this.map.fireEvent("dataload");
                        if (parcelFeatureCollection.features) {
                            let parcelID = parcelFeatureCollection.features[0].properties.ParcelID;
                            if (this.highlightedParcelID != parcelID && this.selectedParcelIDs.some(x => x == parcelID)) {
                                this.highlightedParcelID = parcelID;
                                this.highlightedParcelIDChange.emit(this.highlightedParcelID);
                            }
                        }
                    });
            });
        }
    }

    public highlightParcel() {
        if (this.highlightedParcelLayer) {
            this.map.removeLayer(this.highlightedParcelLayer);
            this.highlightedParcelLayer = null;
        }
        if (this.highlightedParcelID) {
            let wmsParameters = Object.assign({ styles: this.highlightedParcelStyle, cql_filter: `ParcelID = ${this.highlightedParcelID}` }, this.defaultParcelsWMSOptions);
            this.highlightedParcelLayer = tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsParameters);
            this.highlightedParcelLayer.addTo(this.map).bringToFront();
            this.fitBoundsToSelectedParcels([this.highlightedParcelID]);
            this.overlayLayers[this.wellLayerName].bringToFront();
        }
    }

    public setControl(): void {
        var loadingControl = Control.loading({
            separate: true,
            position: 'bottomleft'
        });
        this.map.addControl(loadingControl);

        this.layerControl = new Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        if (this.displayparcelsLayerOnLoad) {
            this.overlayLayers[this.parcelLayerName].addTo(this.map);
            this.overlayLayers[this.wellLayerName].addTo(this.map);
        }
        this.afterSetControl.emit(this.layerControl);
    }
}

