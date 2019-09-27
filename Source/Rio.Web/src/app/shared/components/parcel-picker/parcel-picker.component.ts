import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';

import { Feature, FeatureCollection } from "geojson";
import * as L from "leaflet";
import { GeoJSON, LeafletMouseEvent, TileLayer } from "leaflet";
import { ParcelMapComponent } from '../parcel-map/parcel-map.component';
import { WfsService } from '../../services/wfs.service';
import { ParcelService } from 'src/app/services/parcel/parcel.service';
import { WaterTransferRegistrationParcelDto } from '../../models/water-transfer-registration-parcel-dto';


@Component({
    selector: 'rio-parcel-picker',
    templateUrl: './parcel-picker.component.html',
    styleUrls: ['./parcel-picker.component.scss']
})
export class ParcelPickerComponent implements OnInit, AfterViewInit {

    @Input("visibleParcelIDs")
    public visibleParcelIDs: Array<number> = [];

    @Input("selectedParcels")
    public selectedParcels: Array<WaterTransferRegistrationParcelDto> = [];

    @Input("maxTotalQuantity")
    public maxTotalQuantity: number;

    @Input("defaultExtent")
    public defaultExtent: any;

    @ViewChild("parcelsMap", { static: false })
    public parcelMap: ParcelMapComponent;

    public selectedParcelLayerName: string = 'Selected Parcels';

    constructor(
        private wfsService: WfsService,
        private parcelService: ParcelService
    ) {
    }

    public ngOnInit(): void {
    }

    public ngAfterViewInit(): void {
        const self = this;
        this.parcelMap.map.on("click", (event: LeafletMouseEvent): void => {
            self.wfsService.getParcelByCoordinate(event.latlng.lng, event.latlng.lat)
                .subscribe((parcelFeatureCollection: FeatureCollection) => {
                    parcelFeatureCollection.features
                        .forEach((feature: Feature) => {
                            self.toggleParcel.bind(self)(feature);
                        });
                });
        });
    }

    public getSelectedParcelIDs(): Array<number> {
        return this.selectedParcels !== undefined ? this.selectedParcels.map(p => p.ParcelID) : [];
    }

    public getTotalEntered(): number {
        if (this.selectedParcels.length > 0) {
            let result = this.selectedParcels.reduce(function (a, b) {
                const acreFeet = b.AcreFeetTransferred ? b.AcreFeetTransferred : 0;
                return (a + acreFeet);
            }, 0);
            return result;
        }
        return 0;
    }

    public getQuantityNeeded(): number {
        let totalEntered = this.getTotalEntered();
        return this.maxTotalQuantity - totalEntered;
    }

    public toggleParcel(feature: Feature): void {
        const selectedParcelID = feature.properties.ParcelID;
        if (this.visibleParcelIDs.includes(selectedParcelID)) {
            const selectedParcelIndex = this.selectedParcels.findIndex((parcel: WaterTransferRegistrationParcelDto) =>
                parcel.ParcelID === selectedParcelID);
            if (selectedParcelIndex !== -1) {
                this.selectedParcels.splice(selectedParcelIndex, 1);
                if(this.selectedParcels.length === 1)
                {
                    this.selectedParcels[0].AcreFeetTransferred = this.maxTotalQuantity;
                }
            } else {
                let parcelToAdd = new WaterTransferRegistrationParcelDto();
                parcelToAdd.ParcelID = feature.properties.ParcelID;
                parcelToAdd.ParcelNumber = feature.properties.ParcelNumber;
                if(this.selectedParcels.length === 0)
                {
                    parcelToAdd.AcreFeetTransferred = this.maxTotalQuantity;
                }
                else if(this.selectedParcels.length === 1)
                {
                    this.selectedParcels[0].AcreFeetTransferred = null;
                }
                this.selectedParcels.push(parcelToAdd);
            }
            this.updateSelectedParcelLayer();
        }
    }

    private updateSelectedParcelLayer() {
        this.parcelMap.updateSelectedParcelsOverlayLayer(this.getSelectedParcelIDs());
    }

    public removeParcel(parcelIDToRemove: number): void {
        const selectedParcelIndex = this.selectedParcels.findIndex((parcel: WaterTransferRegistrationParcelDto) =>
            parcel.ParcelID === parcelIDToRemove);
        if (selectedParcelIndex !== -1) {
            this.selectedParcels.splice(selectedParcelIndex, 1);
            if(this.selectedParcels.length === 1)
            {
                this.selectedParcels[0].AcreFeetTransferred = this.maxTotalQuantity;
            }
        }
        this.updateSelectedParcelLayer();
    }
}
