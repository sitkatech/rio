import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Control, FitBoundsOptions, LeafletEvent, map, Map, MapOptions, tileLayer, geoJSON, control, DomUtil } from 'leaflet';
import { BoundingBoxDto } from '../../models/bounding-box-dto';
import { CustomCompileService } from '../../services/custom-compile.service';
import { WfsService } from "../../services/wfs.service";

@Component({
    selector: 'scenario-map',
    templateUrl: './scenario-map.component.html',
    styleUrls: ['./scenario-map.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class ScenarioMapComponent implements OnInit, AfterViewInit {
    @Input()
    public mapID: string = '';

    @Input()
    public zoomMapToDefaultExtent: boolean = true;

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

    public component: any;

    public map: Map;
    public featureLayer: any;
    public layerControl: Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};
    public Sample = require("../../../../assets/WaterTradingScenarioJSON/1981-January2020.json");
    boundingBox: BoundingBoxDto;

    constructor(
        private wfsService: WfsService,
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

        this.compileService.configure(this.appRef);
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

        var fileOptions = JSON.parse(this.Sample.FileDetails);
        var mapPoints = JSON.parse(fileOptions.ResultSets[0].MapData.MapPoints);
        geoJSON(mapPoints, {style:this.setStyle}).addTo(this.map);

        var legend = control({position:'bottomright'});
        legend.onAdd = function(map: any): any {
            var div = DomUtil.create('div', 'legend');
            div.innerHTML = `<div class='legend-title'>
                                    <div class='legend-label'>
                                        <i class='fas fa-arrow-up'></i>
                                    </div>
                                    <div class='legend-units'>
                                        <span>Feet</span>
                                    </div>
                                    <div class='legend-label'>
                                        <i class='fas fa-arrow-down'></i>
                                    </div>
                            </div>`
            
            for (var legendItem of fileOptions.ResultSets[0].MapData.Legend) {
                div.innerHTML += `<div class='legend-item'>
                                        <div class='legend-color' style='background-color:` + legendItem.IncreaseColor + `'></div>
                                        <div class='legend-value'><span class='align-middle'>` + legendItem.Value.toFixed(2) + `</span></div>
                                        <div class='legend-color' style='background-color:` + legendItem.DecreaseColor + `'></div>
                                   </div>`
            }
            return div;
        }

        legend.addTo(this.map);
        this.setControl();
    }

    public setStyle(feature:any): any {
        return {
            color: feature.properties.color,
            weight: 0.5,
            opacity: 0.75,
            fillColor: feature.properties.color,
            fillOpacity: 0.75
        }
    }

    public setControl(): void {
        this.layerControl = new Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        this.afterSetControl.emit(this.layerControl);
    }
}

