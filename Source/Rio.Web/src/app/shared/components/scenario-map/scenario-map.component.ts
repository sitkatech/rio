import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import * as L from 'leaflet';
import '../../../../../node_modules/leaflet-timedimension/dist/leaflet.timedimension.src.js';
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
    public defaultFitBoundsOptions?: L.FitBoundsOptions = null;

    @Output()
    public afterSetControl: EventEmitter<L.Control.Layers> = new EventEmitter();

    @Output()
    public afterLoadMap: EventEmitter<L.LeafletEvent> = new EventEmitter();

    @Output()
    public onMapMoveEnd: EventEmitter<L.LeafletEvent> = new EventEmitter();

    public component: any;

    public map: L.Map;
    public featureLayer: any;
    public layerControl: L.Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};
    public January2020 = require("../../../../assets/WaterTradingScenarioJSON/1981-January2020.json");
    public February2020 = require("../../../../assets/WaterTradingScenarioJSON/1981-February2020.json");
    public March2020 = require("../../../../assets/WaterTradingScenarioJSON/1981-March2020.json");
    public AvailableMonths = [this.January2020, this.February2020, this.March2020];
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
            "Aerial": L.tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Aerial',
            }),
            "Street": L.tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Aerial',
            }),
            "Terrain": L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
                attribution: 'Terrain',
            }),
        }, this.tileLayers);

        this.compileService.configure(this.appRef);
    }

    public ngAfterViewInit(): void {

        const mapOptions: L.MapOptions = {
            // center: [46.8797, -110],
            // zoom: 6,
            minZoom: 6,
            maxZoom: 17,
            layers: [
                this.tileLayers["Aerial"],
            ],
            timeDimension: true,
            timeDimensionOptions: {
                timeInterval: "2020-01-01/2020-03-01",
                period: "P1M"
            },
            timeDimensionControl: true
        } as L.MapOptions;
        this.map = L.map(this.mapID, mapOptions);

        this.map.on('load', (event: L.LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: L.LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
        });
        this.map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);

        var mapObj = {
            "type":"FeatureCollection",
            "features": new Array
        };

        for (var file of this.AvailableMonths)
        {
            var fileOptions = JSON.parse(file.FileDetails);
            var date = fileOptions.RunResultName;
            var mapPoints = JSON.parse(fileOptions.ResultSets[0].MapData.MapPoints);
            for (var mapPoint of mapPoints.features)
            {
                mapPoint.properties["time"] = new Date(date + " UTC").toISOString();
                mapObj.features.push(mapPoint);
            }
        }
        
        var geoJSONLayer = L.geoJSON(mapObj, {style:this.setStyle});
        var geoJSONTDLayer = L.timeDimension.layer.geoJson(geoJSONLayer);
        //geoJSONLayer.addTo(this.map);
        geoJSONTDLayer.addTo(this.map);

        var legend = L.control({position:'bottomright'});
        legend.onAdd = function(map: any): any {
            var div = L.DomUtil.create('div', 'legend');
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
        this.layerControl = new L.Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        this.afterSetControl.emit(this.layerControl);
    }
}

