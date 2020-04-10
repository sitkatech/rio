import { AfterViewInit, ApplicationRef, ChangeDetectionStrategy, Component, EventEmitter, OnInit} from '@angular/core';
import * as L from 'leaflet';
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


  public disadvantagedCommunityStyle = 'scenario_disadvantagedcommunity';
  public waterTradingScenarioWellStyle = 'scenario_watertradingscenariowells';
  public allParcelsStyle = 'parcel_pink';

  boundingBox: BoundingBoxDto;

  constructor(
      private appRef: ApplicationRef,
      private compileService: CustomCompileService
  ) {
  }

  public ngOnInit(): void {
      // Default bounding box
      this.boundingBox = new BoundingBoxDto();
      this.boundingBox.Left = -119.17;
      this.boundingBox.Bottom = 35.442022035628575;
      this.boundingBox.Right = -119.51;
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

      this.defaultParcelsWMSOptions = ({
          layers: "Rio:AllParcels",
          transparent: true,
          format: "image/png",
          tiled: true
      } as L.WMSOptions);

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

      let allParcelsOptions = Object.assign({styles:this.allParcelsStyle}, this.defaultParcelsWMSOptions);
      let disadvantagedCommunityOptions = Object.assign({styles:this.disadvantagedCommunityStyle}, this.defaultDisadvantagedCommunitiesOptions);
      let waterTradingScenarioWellOptions = Object.assign({styles:this.waterTradingScenarioWellStyle}, this.defaultWaterTradingScenarioWellOptions);
      
      this.overlayLayers = Object.assign({
          "All Parcels": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", allParcelsOptions),
          "<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged Communities": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", disadvantagedCommunityOptions),
          "<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells": L.tileLayer.wms(environment.geoserverMapServiceUrl + "/wms?", waterTradingScenarioWellOptions)
      }, this.overlayLayers);
      

      this.compileService.configure(this.appRef);
  }

  public ngAfterViewInit(): void {

      const mapOptions: L.MapOptions = {
          minZoom: 6,
          maxZoom: 17,
          layers: [
              this.tileLayers["Terrain"],
              this.overlayLayers["All Parcels"],
              this.overlayLayers["<img src='../../../assets/main/images/disadvantaged_community.png' style='height:16px'> Disadvantaged Communities"],
              this.overlayLayers["<img src='../../../assets/main/images/water_trading_scenario_well.png' style='height:16px'> Wells"]
          ]
      } as L.MapOptions;
      this.map = L.map(this.mapID, mapOptions);

      this.map.on('load', (event: L.LeafletEvent) => {
          this.afterLoadMap.emit(event);
      });
      this.map.on("moveend", (event: L.LeafletEvent) => {
          this.onMapMoveEnd.emit(event);
      });

      this.map.fitBounds([[this.boundingBox.Bottom, this.boundingBox.Left], [this.boundingBox.Top, this.boundingBox.Right]], this.defaultFitBoundsOptions);

      this.setControl();
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
}
