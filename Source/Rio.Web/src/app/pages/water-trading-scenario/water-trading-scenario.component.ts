import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, OnInit} from '@angular/core';
import * as L from 'leaflet';
import '../../../../node_modules/leaflet-timedimension/dist/leaflet.timedimension.src.js';
import { BoundingBoxDto } from '../../shared/models/bounding-box-dto';
import { CustomCompileService } from '../../shared/services/custom-compile.service';

@Component({
    templateUrl: './water-trading-scenario.component.html',
    styleUrls: ['./water-trading-scenario.component.scss'],
    changeDetection: ChangeDetectionStrategy.OnPush
})
export class WaterTradingScenarioComponent implements OnInit, AfterViewInit {
    
    public zoomMapToDefaultExtent = true;
    public defaultFitBoundsOptions?: L.FitBoundsOptions = null;
    public afterSetControl = new EventEmitter();
    public afterLoadMap = new EventEmitter();
    public onMapMoveEnd = new EventEmitter();

    public component: any;

    public mapID = "WaterTradingScenarioMap";
    public mapHeight = "600px";
    public map: L.Map;
    public featureLayer: any;
    public layerControl: L.Control.Layers;
    public tileLayers: { [key: string]: any } = {};
    public overlayLayers: { [key: string]: any } = {};
    public January2020 = require("../../../assets/WaterTradingScenarioJSON/1981-January2020.json");
    public February2020 = require("../../../assets/WaterTradingScenarioJSON/1981-February2020.json");
    public March2020 = require("../../../assets/WaterTradingScenarioJSON/1981-March2020.json");
    public April2020 = require("../../../assets/WaterTradingScenarioJSON/1981-April2020.json");
    public May2020 = require("../../../assets/WaterTradingScenarioJSON/1981-May2020.json");
    public June2020 = require("../../../assets/WaterTradingScenarioJSON/1981-June2020.json");
    public July2020 = require("../../../assets/WaterTradingScenarioJSON/1981-July2020.json");
    public August2020 = require("../../../assets/WaterTradingScenarioJSON/1981-August2020.json");
    public September2020 = require("../../../assets/WaterTradingScenarioJSON/1981-September2020.json");
    public October2020 = require("../../../assets/WaterTradingScenarioJSON/1981-October2020.json");
    public November2020 = require("../../../assets/WaterTradingScenarioJSON/1981-November2020.json");
    public December2020 = require("../../../assets/WaterTradingScenarioJSON/1981-December2020.json");
    public AvailableMonths = [
        this.January2020,
        this.February2020, 
        this.March2020,
        this.April2020,
        this.May2020,
        this.June2020,
        this.July2020,
        this.August2020,
        this.September2020,
        this.October2020,
        this.November2020,
        this.December2020
    ];
    boundingBox: BoundingBoxDto;

    constructor(
        private appRef: ApplicationRef,
        private compileService: CustomCompileService
    ) {
    }

    public ngOnInit(): void {
        // Default bounding box
        this.boundingBox = new BoundingBoxDto();
        this.boundingBox.Left = -119.075;
        this.boundingBox.Bottom = 35.442022035628575;
        this.boundingBox.Right = -119.425;
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
            minZoom: 6,
            maxZoom: 17,
            layers: [
                this.tileLayers["Terrain"],
            ],
            timeDimension: true,
            timeDimensionOptions: {
                timeInterval: "2020-01-01/2020-12-01",
                period: "P1M",
                currentTime: Date.parse("2020-01-01 UTC")
            },
            timeDimensionControl: true,
            timeDimensionControlOptions: {
                loopButton: true
            }
        } as L.MapOptions;
        this.map = L.map(this.mapID, mapOptions);

        this.map.on('load', (event: L.LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });
        this.map.on("moveend", (event: L.LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
        });
        this.map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);

        var layer = {
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
                layer.features.push(mapPoint);
            }
        }
        
        var geoJSONLayer = L.geoJSON(layer, {style:this.setStyle});
        var geoJSONTDLayer = L.timeDimension.layer.geoJson(geoJSONLayer, {
            updateTimeDimension: true,
            duration: 'P1D',
            updateTimeDimensionMode: 'replace'
        });
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
                            </div>`;
            
            for (var legendItem of fileOptions.ResultSets[0].MapData.Legend) {
                div.innerHTML += `<div class='legend-item'>
                                        <div class='legend-color' style='background-color:` + legendItem.IncreaseColor + `'></div>
                                        <div class='legend-value'><span class='align-middle'>` + legendItem.Value.toFixed(2) + `</span></div>
                                        <div class='legend-color' style='background-color:` + legendItem.DecreaseColor + `'></div>
                                   </div>`;
            }
            return div;
        }

        legend.addTo(this.map);
        this.setControl();
        
    }

    public setStyle(feature:any): any {
        return {
            color: feature.properties.color,
            weight: 0
        }
    }

    public setControl(): void {
        this.layerControl = new L.Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        this.afterSetControl.emit(this.layerControl);
    }
}

