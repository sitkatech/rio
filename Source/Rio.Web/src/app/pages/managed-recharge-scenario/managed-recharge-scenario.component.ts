import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, OnInit} from '@angular/core';
import * as L from 'leaflet';
import '../../../../node_modules/leaflet-timedimension/dist/leaflet.timedimension.src.js';
import { BoundingBoxDto } from '../../shared/models/bounding-box-dto';
import { CustomCompileService } from '../../shared/services/custom-compile.service';
import { environment } from "src/environments/environment";

declare var $ : any;

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

  public mapID = "ManagedRechargeScenarioMap";
  public mapHeight = "600px";
  public map: L.Map;
  public featureLayer: any;
  public layerControl: L.Control.Layers;
  public tileLayers: { [key: string]: any } = {};
  public overlayLayers: { [key: string]: any } = {};
  private defaultParcelsWMSOptions: L.WMSOptions;
  private defaultDisadvantagedCommunitiesOptions: L.WMSOptions;
  private defaultWaterTradingScenarioWellOptions: L.WMSOptions;
  private defaultScenarioArsenicContaminationOptions: L.WMSOptions;
  private defaultScenarioRechargeBasinOptions: L.WMSOptions;
  public availableScenarioInfo = require('../../../assets/ManagedRechargeScenarioJSON/availableRunInfo_2073.json');

  public disadvantagedCommunityStyle = 'scenario_disadvantagedcommunity';
  public waterTradingScenarioWellStyle = 'scenario_watertradingscenariowells';
  public arsenicContaminationStyle = 'point_circle_red';
  public rechargeBasinStyle = 'scenario_rechargebasin';

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
      this.boundingBox.Left = -118.88031005859376;
      this.boundingBox.Bottom = 35.17324704631253;
      this.boundingBox.Right = -119.61502075195314;
      this.boundingBox.Top = 35.50931279638352;

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

      this.defaultDisadvantagedCommunitiesOptions = ({
          layers: "Rio:DisadvantagedCommunity",
          transparent: true,
          format: "image/png",
          tiled: true
      } as L.WMSOptions);

      this.defaultWaterTradingScenarioWellOptions = ({
          layers: "Rio:Wells",
          transparent: true,
          format: "image/png",
          tiled: true
      } as L.WMSOptions);

      this.defaultScenarioArsenicContaminationOptions = ({
          layers: "Rio:ScenarioArsenicContamination",
          transparent: true,
          format: "image/png",
          tiled: true
      } as L.WMSOptions);

      this.defaultScenarioRechargeBasinOptions = ({
          layers: "Rio:ScenarioRechargeBasin",
          transparent: true,
          format: "image/png",
          tiled: true
      } as L.WMSOptions);

      let disadvantagedCommunityOptions = Object.assign({styles:this.disadvantagedCommunityStyle}, this.defaultDisadvantagedCommunitiesOptions);
      let waterTradingScenarioWellOptions = Object.assign({styles:this.waterTradingScenarioWellStyle}, this.defaultWaterTradingScenarioWellOptions);
      let scenarioArsenicContaminationOptions = Object.assign({styles:this.arsenicContaminationStyle}, this.defaultScenarioArsenicContaminationOptions);
      let scenarioRechargeBasin = Object.assign({styles:this.rechargeBasinStyle}, this.defaultScenarioRechargeBasinOptions);
      
      this.overlayLayers = Object.assign({
        "<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged Communities": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", disadvantagedCommunityOptions),
        "<img src='../../../assets/main/images/scenario_recharge_basin.png' style='height:16px'> Recharge Basins": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", scenarioRechargeBasin),
        "<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", waterTradingScenarioWellOptions),
        "<img src='../../../assets/main/images/scenario_arsenic_contamination.png' style='height:16px'> Monitored Arsenic Contamination (> 10&micro;g/L)": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", scenarioArsenicContaminationOptions)
      }, this.overlayLayers);
      

      this.compileService.configure(this.appRef);
  }

  public ngAfterViewInit(): void {

    var firstRunObject = this.availableScenarioInfo[0];
    var firstRunDate = this.getISOString(firstRunObject.RunDate);
    var lastRunObject = this.availableScenarioInfo[this.availableScenarioInfo.length - 1];
    var lastRunDate = this.getISOString(lastRunObject.RunDate);

    const mapOptions: L.MapOptions = {
        minZoom: 6,
        maxZoom: 17,
        layers: [
            this.tileLayers["Terrain"],
            this.overlayLayers["<img src='../../../assets/main/images/scenario_arsenic_contamination.png' style='height:16px'> Monitored Arsenic Contamination (> 10&micro;g/L)"],
            this.overlayLayers["<img src='../../../assets/main/images/scenario_recharge_basin.png' style='height:16px'> Recharge Basins"],
            this.overlayLayers["<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged Communities"],
            this.overlayLayers["<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells"]
        ],
        timeDimension: true,
            timeDimensionOptions: {
                timeInterval: firstRunDate + "/" + lastRunDate, //this.getISOString(JSON.parse(firstRunObject.FileDetails).RunResultName) + "/" + this.getISOString(JSON.parse(lastRunObject.FileDetails).RunResultName),
                period: "P1M",
                currentTime: Date.parse(firstRunDate) //Date.parse(this.getISOString(JSON.parse(firstRunObject.FileDetails).RunResultName))
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
        var map = event.target;
        console.log(map.getBounds());
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

    var legendItems = firstRunObject.Legend;  //JSON.parse(firstRunObject.FileDetails);  
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
    $("g").css("opacity", 0.4);
  }

  public setStyle(feature:any): any {
      return {
          color: feature.properties.color,
          stroke: false,
          fillOpacity: 1
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
