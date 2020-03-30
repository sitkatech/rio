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
    
    public interval = 0;
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
    public availableScenarioInfo = require('../../../assets/WaterTradingScenarioJSON/availableRunInfo.json');

    public months = {
        'January' : '01',
        'February' : '02',
        'March' : '03',
        'April' : '04',
        'May' : '05',
        'June' : '06',
        'July' : '07',
        'August' : '08',
        'September' : '09',
        'October' : '10',
        'November' : '11',
        'December' : '12'
    }

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
                timeInterval: this.getISOString(this.availableScenarioInfo[0].RunDate) + "/" + this.getISOString(this.availableScenarioInfo[this.availableScenarioInfo.length-1].RunDate), //this.getISOString(JSON.parse(this.availableScenarioInfo[0].FileDetails).RunResultName) + "/" + this.getISOString(JSON.parse(this.availableScenarioInfo[this.availableScenarioInfo.length-1].FileDetails).RunResultName),
                period: "P1M",
                currentTime: Date.parse(this.getISOString(this.availableScenarioInfo[0].RunDate)) //Date.parse(this.getISOString(JSON.parse(this.availableScenarioInfo[0].FileDetails).RunResultName))
            },
            timeDimensionControl: true,
            timeDimensionControlOptions: {
                loopButton: true,
                autoPlay: true,
                playerOptions: {
                    loop: true
                },
                timeZones: ["Local"]
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

        for (var file of this.availableScenarioInfo)
        {
            //var fileOptions = JSON.parse(file.FileDetails);
            var mapPoints = JSON.parse(file.RunFeatures); //JSON.parse(fileOptions.ResultSets[0].MapData.MapPoints);
            var time = this.getISOString(file.RunDate); //this.getISOString(fileOptions.RunResultName);
            for (var mapPoint of mapPoints.features)
            {
                mapPoint.properties["time"] = time;
                if (mapPoint.geometry.type == "MultiPolygon")
                {
                    for (var coords of mapPoint.geometry.coordinates)
                    {
                        var newGeometry = {...mapPoint.geometry};
                        var splitMultiPolygon = {...mapPoint};
                        newGeometry.type = "Polygon";
                        newGeometry.coordinates = coords;
                        splitMultiPolygon.geometry = newGeometry;
                        layer.features.push(splitMultiPolygon);
                    }
                }
                else
                {
                    layer.features.push(mapPoint);
                }
            }
        }
        
        var geoJSONLayer = L.geoJSON(layer, {style:this.setStyle});
        //geoJSONLayer.addTo(this.map);
        var geoJSONTDLayer = L.timeDimension.layer.geoJson(geoJSONLayer, {
            duration:"PT1M"
        });
        geoJSONTDLayer.addTo(this.map);

        var legendItems = this.availableScenarioInfo[0].Legend;  //JSON.parse(this.availableScenarioInfo[0].FileDetails);  
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
                            
                      
            for (var legendItem of legendItems /*legendItems.ResultSets[0].MapData.Legend*/) {
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
            stroke: false
        }
    }

    public setControl(): void {
        this.layerControl = new L.Control.Layers(this.tileLayers, this.overlayLayers)
            .addTo(this.map);
        this.afterSetControl.emit(this.layerControl);
    }

    public getISOString(fileDate: string): string {
        var contents = fileDate.split(" ");
        return new Date(contents[1] + "-" + this.months[contents[0]] + "-01").toISOString();
    }
}

