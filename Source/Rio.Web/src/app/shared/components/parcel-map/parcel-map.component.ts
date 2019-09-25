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
    public visibleParcelIDs: Array<number> = [];

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

    @Input()
    public selectedParcelLayerName: string = 'Selected Parcels';

    @Output()
    public afterSetControl: EventEmitter<Control.Layers> = new EventEmitter();

    @Output()
    public afterLoadMap: EventEmitter<LeafletEvent> = new EventEmitter();

    @Output()
    public onMapMoveEnd: EventEmitter<LeafletEvent> = new EventEmitter();

    public component: any;

    public map: Map;
    public featureLayer: any;
    public layerControl: Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};
    boundingBox: BoundingBoxDto;
    private selectedParcelLayer: any;

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

        let parcelsWMSOptions = ({
            layers: "Rio:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true,
        } as WMSOptions);
        if (this.visibleParcelIDs.length > 0) {
            parcelsWMSOptions.cql_filter = this.createParcelMapFilter(this.visibleParcelIDs);
            this.fitBoundsToSelectedParcels(this.visibleParcelIDs);
        }
        this.overlayLayers = Object.assign({
            "Parcels": tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", parcelsWMSOptions)
        }, this.overlayLayers);

        this.compileService.configure(this.appRef);
    }

    public updateSelectedParcelsOverlayLayer(parcelIDs: Array<number>) {
        if (this.selectedParcelLayer) {
            this.layerControl.removeLayer(this.selectedParcelLayer);
            this.map.removeLayer(this.selectedParcelLayer);
        }

        var wmsParameters = {
            layers: "Rio:AllParcels",
            transparent: true,
            format: "image/png",
            tiled: true,
            styles: "parcel_yellow",
            cql_filter: this.createParcelMapFilter(parcelIDs)
        };

        this.selectedParcelLayer = tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", wmsParameters);
        this.layerControl.addOverlay(this.selectedParcelLayer, this.selectedParcelLayerName);

        this.selectedParcelLayer.addTo(this.map).bringToFront();
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
    }

    public setControl(): void {
        this.layerControl = new Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        if (this.displayparcelsLayerOnLoad) {
            this.overlayLayers["Parcels"].addTo(this.map);
        }
        this.afterSetControl.emit(this.layerControl);
    }
}

