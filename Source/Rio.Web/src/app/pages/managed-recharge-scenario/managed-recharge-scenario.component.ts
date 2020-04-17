import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, OnInit } from '@angular/core';
import * as L from 'leaflet';
import '../../../../node_modules/leaflet-timedimension/dist/leaflet.timedimension.src.js';
import { BoundingBoxDto } from '../../shared/models/bounding-box-dto';
import { CustomCompileService } from '../../shared/services/custom-compile.service';
import { environment } from "src/environments/environment";

declare var $: any;

@Component({
    selector: 'rio-managed-recharge-scenario',
    templateUrl: './managed-recharge-scenario.component.html',
    styleUrls: ['./managed-recharge-scenario.component.scss']
})
export class ManagedRechargeScenarioComponent implements OnInit {


    public interval = 0;
    public zoomMapToDefaultExtent = true;
    public defaultFitBoundsOptions?: L.FitBoundsOptions = null;
    public afterSetControl = new EventEmitter();
    public afterLoadMap = new EventEmitter();
    public onMapMoveEnd = new EventEmitter();

    public component: any;

    public eastMapID = "ManagedRechargeScenarioMapEastern";
    public westMapID = "ManagedRechargeScenarioMapWestern";
    public mapHeight = "450px";
    public eastScenarioMap: L.Map;
    public westScenarioMap: L.map;
    public featureLayer: any;
    public tileLayersArray: any = [];
    public overlayLayersArray: any = [];
    public arrayPosition: number = 0;
    public westernScenarioInfo = require('../../../assets/ManagedRechargeScenarioJSON/westernScenario_May2021_2093.json');
    public easternScenarioInfo = require('../../../assets/ManagedRechargeScenarioJSON/easternScenario_May2021_2105.json');

    public disadvantagedCommunityStyle = 'scenario_disadvantagedcommunity';
    public waterTradingScenarioWellStyle = 'scenario_watertradingscenariowells';
    public arsenicContaminationLocationStyle = 'point_circle_red';
    public rechargeBasinStyle = 'scenario_rechargebasin';

    boundingBox: BoundingBoxDto;

    constructor(
        private appRef: ApplicationRef,
        private compileService: CustomCompileService
    ) {
    }

    public ngOnInit(): void {
        // Default bounding box
        this.boundingBox = new BoundingBoxDto();
        this.boundingBox.Left = -119.45228576660158;
        this.boundingBox.Bottom = 35.237207054596404;
        this.boundingBox.Right = -119.11033630371095;
        this.boundingBox.Top = 35.46122923189147;

        const getTileLayers = () => {
            let tileLayer = {};
            tileLayer = Object.assign({}, {
                "Aerial": L.tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}', {
                    attribution: 'Aerial',
                }),
                "Street": L.tileLayer('https://services.arcgisonline.com/ArcGIS/rest/services/World_Street_Map/MapServer/tile/{z}/{y}/{x}', {
                    attribution: 'Aerial',
                }),
                "Terrain": L.tileLayer('https://server.arcgisonline.com/ArcGIS/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}', {
                    attribution: 'Terrain',
                }),
            }, tileLayer);

            this.tileLayersArray.push(tileLayer);
        }

        const getOverlayLayers = () => {

            let defaultDisadvantagedCommunitiesOptions = ({
                layers: "Rio:DisadvantagedCommunity",
                transparent: true,
                format: "image/png",
                tiled: true
            } as L.WMSOptions);

            let defaultWaterTradingScenarioWellOptions = ({
                layers: "Rio:Wells",
                transparent: true,
                format: "image/png",
                tiled: true
            } as L.WMSOptions);

            let defaultScenarioArsenicContaminationLocationOptions = ({
                layers: "Rio:ScenarioArsenicContaminationLocation",
                transparent: true,
                format: "image/png",
                tiled: true
            } as L.WMSOptions);

            let defaultScenarioRechargeBasinOptions = ({
                layers: "Rio:ScenarioRechargeBasin",
                transparent: true,
                format: "image/png",
                tiled: true
            } as L.WMSOptions);

            let disadvantagedCommunityOptions = Object.assign({ styles: this.disadvantagedCommunityStyle }, defaultDisadvantagedCommunitiesOptions);
            let waterTradingScenarioWellOptions = Object.assign({ styles: this.waterTradingScenarioWellStyle }, defaultWaterTradingScenarioWellOptions);
            let scenarioArsenicContaminationLocationOptions = Object.assign({ styles: this.arsenicContaminationLocationStyle }, defaultScenarioArsenicContaminationLocationOptions);
            let scenarioRechargeBasin = Object.assign({ styles: this.rechargeBasinStyle }, defaultScenarioRechargeBasinOptions);

            let overlayLayers = {};
            overlayLayers = Object.assign({
                "<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged <br/> Communities": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", disadvantagedCommunityOptions),
                "<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", waterTradingScenarioWellOptions),
                "<img src='../../../assets/main/images/scenario_arsenic_contamination.png' style='height:16px'> Monitored Arsenic <br/> Contamination (> 10&micro;g/L)": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", scenarioArsenicContaminationLocationOptions),
                "<img src='../../../assets/main/images/scenario_recharge_basin.png' style='height:16px'> Recharge Basins": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", scenarioRechargeBasin)
            }, overlayLayers);

            this.overlayLayersArray.push(overlayLayers);
        }

        //Layers in leaflet are not allowed to be reused, so we'll
        //make arrays of tile and overlay layers
        [this.eastScenarioMap, this.westScenarioMap].forEach(() => {
            getTileLayers();
            getOverlayLayers();
        });

        this.compileService.configure(this.appRef);
    }

    public ngAfterViewInit(): void {

        [{map:this.westScenarioMap, id: this.westMapID, scenario: this.westernScenarioInfo},
         {map:this.eastScenarioMap, id: this.eastMapID, scenario: this.easternScenarioInfo}].forEach(
             (x) => {
                 x.map = this.initializeMap(x.map, x.id);
                 this.addScenarioToMap(x.map, x.scenario);
                 this.addLegendToMap(x.map);
             });
    }

    public initializeMap(map: L.map, mapID: string): L.map {

        const mapOptions: L.MapOptions = {
            minZoom: 6,
            maxZoom: 17,
            layers: [
                this.tileLayersArray[this.arrayPosition]["Terrain"],
                this.overlayLayersArray[this.arrayPosition]["<img src='../../../assets/main/images/scenario_arsenic_contamination.png' style='height:16px'> Monitored Arsenic <br/> Contamination (> 10&micro;g/L)"],
                this.overlayLayersArray[this.arrayPosition]["<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells"],
                this.overlayLayersArray[this.arrayPosition]["<img src='../../../assets/main/images/scenario_recharge_basin.png' style='height:16px'> Recharge Basins"],
                this.overlayLayersArray[this.arrayPosition]["<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged <br/> Communities"]
                
            ]
        } as L.MapOptions;

        map = L.map(mapID, mapOptions);

        map.on('load', (event: L.LeafletEvent) => {
            this.afterLoadMap.emit(event);
        });

        map.on("moveend", (event: L.LeafletEvent) => {
            this.onMapMoveEnd.emit(event);
            var map = event.target;
            console.log(map.getBounds());
        });

        map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);


        new L.Control.Layers(this.tileLayersArray[this.arrayPosition], this.overlayLayersArray[this.arrayPosition], {collapsed:false}).addTo(map);

        this.addCloseButton(map);

        this.arrayPosition = this.arrayPosition + 1;

        return map;
    }

    public setStyle(feature: any): any {
        return {
            color: feature.properties.color,
            fillOpacity: 0.6,
            stroke:false,
            smoothFactor: 0.3
        }
    }

    public addCloseButton(map: L.Map): void {
        let mapContainer = $(map._container);
        let closeButtonClass = "leaflet-control-layers-close";
        let leafletControlLayersSelector = $(mapContainer.find(".leaflet-control-layers"));

        let closeButton = L.DomUtil.create("a", closeButtonClass);
        closeButton.innerHTML = "Close";       
        leafletControlLayersSelector.append(closeButton);
       
        let toggleSelector = $(mapContainer.find(".leaflet-control-layers-toggle"));
        let closeButtonSelector = $(mapContainer.find(".leaflet-control-layers-close"));

        L.DomEvent.on(closeButton, "click", function(e) {
            leafletControlLayersSelector.removeClass("leaflet-control-layers-expanded");
            closeButtonSelector.toggle();
        });

        toggleSelector.on("click", function () {
            closeButtonSelector.toggle()
        });

        map.closeMapLayersControl = function() {
            leafletControlLayersSelector.removeClass("leaflet-control-layers-expanded");
            closeButtonSelector.toggle();
        }

        map.closeMapLayersControl();
    }

    public addScenarioToMap(map: L.map, scenario:any): void {
        let fileOptions = JSON.parse(scenario.FileDetails);
        let features = JSON.parse(fileOptions.ResultSets[0].MapData.MapPoints);
        let geoJSONLayer = L.geoJSON(features, {style:this.setStyle});
        geoJSONLayer.addTo(map);
    }

    public addLegendToMap(map: L.map): void {
        var legendItems = JSON.parse(this.westernScenarioInfo.FileDetails);  
        var legend = L.control({position:'bottomleft'});
        legend.onAdd = function(map: any): any {
            var div = L.DomUtil.create('div', 'legend');
            div.innerHTML = `<div class='legend-title'>
                                    <div class='legend-label'>
                                        <i class='fas fa-arrow-up'></i>
                                    </div>
                                    <div class='legend-units'>
                                        <span>Feet</span>
                                    </div>
                            </div>`;


            for (var legendItem of legendItems.ResultSets[0].MapData.Legend) {
                div.innerHTML += `<div class='legend-item'>
                                        <div class='legend-color' style='background-color:` + legendItem.IncreaseColor + `'></div>
                                        <div class='legend-value'><span class='align-middle'>` + legendItem.Value.toFixed(2) + `</span></div>
                                    </div>`;
            }
            return div;
        }

        legend.addTo(map);
    }
}
