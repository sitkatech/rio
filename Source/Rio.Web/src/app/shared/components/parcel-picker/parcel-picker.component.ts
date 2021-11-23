import { AfterViewInit, Component, Input, OnInit, ViewChild, Output, EventEmitter } from '@angular/core';

import { Feature, FeatureCollection } from "geojson";
import * as L from "leaflet";
import { GeoJSON, LeafletMouseEvent, TileLayer } from "leaflet";
import { ParcelMapComponent } from '../parcel-map/parcel-map.component';
import { WfsService } from '../../services/wfs.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { ParcelDto } from '../../generated/model/parcel-dto';
import { WaterTransferRegistrationParcelDto } from '../../generated/model/water-transfer-registration-parcel-dto';


@Component({
    selector: 'rio-parcel-picker',
    templateUrl: './parcel-picker.component.html',
    styleUrls: ['./parcel-picker.component.scss']
})
export class ParcelPickerComponent implements OnInit, AfterViewInit {

    @Input("visibleParcels")
    public visibleParcels: Array<ParcelDto> = [];

    @Input("selectedParcels")
    public selectedParcels: Array<WaterTransferRegistrationParcelDto> = [];

    @Input("maxTotalQuantity")
    public maxTotalQuantity: number;

    @Input("defaultExtent")
    public defaultExtent: any;

    @ViewChild("parcelsMap")
    public parcelMap: ParcelMapComponent;

    public selectedParcelLayerName: string = 'Selected Parcels';
    public visibleParcelIDs: Array<number> = [];
    public isLoadingSubmit: boolean = false;
    public parcelIDsAlreadyAccountedForWithClick = [];

    @Output() onSubmit = new EventEmitter();

    constructor(
        private wfsService: WfsService,
        private parcelService: ParcelService
    ) {
    }

    public ngOnInit(): void {
        this.visibleParcelIDs = this.visibleParcels.map(p => p.ParcelID);
    }

    public ngAfterViewInit(): void {
        const self = this;
        this.parcelMap.map.on("click", (event: LeafletMouseEvent): void => {
            self.wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat)
                .subscribe((parcelFeatureCollection: FeatureCollection) => {
                    this.parcelIDsAlreadyAccountedForWithClick = [];
                    parcelFeatureCollection.features
                        .forEach((feature: Feature) => {
                            //In some cases there are duplicate parcels being returned
                            //This is a temporary fix, but it might be something bigger with geoserver
                            if (this.parcelIDsAlreadyAccountedForWithClick.includes(feature.properties.ParcelID)) {
                                return;
                            }

                            self.toggleParcel.bind(self)(feature);
                            this.parcelIDsAlreadyAccountedForWithClick.push(feature.properties.ParcelID);
                        });
                });
        });
    }

    public getSelectedParcelIDs(): Array<number> {
        return this.selectedParcels !== undefined ? this.selectedParcels.map(p => p.ParcelID) : [];
    }

    public getTotalParcelAreaAcres(): number {
        if (this.selectedParcels.length > 0) {
            let result = this.selectedParcels.reduce(function (a, b) {
                const acreFeet = b.ParcelAreaInAcres ? b.ParcelAreaInAcres : 0;
                return (a + acreFeet);
            }, 0);
            return result;
        }
        return 0;
    }

    public toggleParcel(feature: Feature): void {
        const selectedParcelID = feature.properties.ParcelID;
        if (this.visibleParcelIDs.includes(selectedParcelID)) {
            if(!this.removeParcelIfSelected(selectedParcelID))
            {
                let parcelToAdd = new WaterTransferRegistrationParcelDto();
                parcelToAdd.ParcelID = feature.properties.ParcelID;
                parcelToAdd.ParcelNumber = feature.properties.ParcelNumber;
                parcelToAdd.ParcelAreaInAcres = feature.properties.ParcelAreaInAcres;
                this.selectedParcels.push(parcelToAdd);
            }
            this.updateSelectedParcelLayer();
        }
    }

    public removeParcel(parcelIDToRemove: number): void {
        this.removeParcelIfSelected(parcelIDToRemove);
        this.updateSelectedParcelLayer();
    }

    public removeAllParcels(): void {
        this.selectedParcels = [];
        this.updateSelectedParcelLayer();
    }

    public selectAllParcels(): void {
        this.selectedParcels = this.visibleParcels.map(p => {
            let parcelToAdd = new WaterTransferRegistrationParcelDto();
            parcelToAdd.ParcelID = p.ParcelID;
            parcelToAdd.ParcelNumber = p.ParcelNumber;
            parcelToAdd.ParcelAreaInAcres = p.ParcelAreaInAcres;
            return parcelToAdd
        });
        this.updateSelectedParcelLayer();
    }

    private removeParcelIfSelected(parcelIDToRemove: number) : boolean {
        const selectedParcelIndex = this.selectedParcels.findIndex((parcel: WaterTransferRegistrationParcelDto) => parcel.ParcelID === parcelIDToRemove);
        if (selectedParcelIndex !== -1) {
            this.selectedParcels.splice(selectedParcelIndex, 1);
            return true;
        }
        return false;
    }

    private updateSelectedParcelLayer() {
        this.parcelMap.updateSelectedParcelsOverlayLayer(this.getSelectedParcelIDs());
        this.calculateAreaTransferredForParcels();
    }

    private calculateAreaTransferredForParcels() {
        const totalSelectedParcelArea = this.getTotalParcelAreaAcres();
        this.selectedParcels.forEach(p => {
            p.AcreFeetTransferred = p.ParcelAreaInAcres / totalSelectedParcelArea * this.maxTotalQuantity;
        });
    }

    public isParcelPickerValid(): boolean {
        return this.selectedParcels.length > 0;
    }    

    public onClickNext() {
        this.onSubmit.emit();
    } 
}
