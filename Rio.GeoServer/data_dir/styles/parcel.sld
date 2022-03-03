<?xml version="1.0" encoding="ISO-8859-1"?>
<StyledLayerDescriptor version="1.0.0"
  xsi:schemaLocation="http://www.opengis.net/sld http://schemas.opengis.net/sld/1.0.0/StyledLayerDescriptor.xsd"
  xmlns="http://www.opengis.net/sld" xmlns:ogc="http://www.opengis.net/ogc"
  xmlns:xlink="http://www.w3.org/1999/xlink" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">

    <NamedLayer>
        <Name>all_parcels</Name>
        <UserStyle>
            <Title>Parcel</Title>
            <FeatureTypeStyle>
                <Rule>
                    <Name>Border</Name>
                    <MaxScaleDenominator>2000000</MaxScaleDenominator>
                    <PolygonSymbolizer>
                        <Fill>
                            <CssParameter name="fill">#dddddd</CssParameter>
                            <CssParameter name="fill-opacity">0.1</CssParameter>
                        </Fill>
                        <Stroke>
                            <CssParameter name="stroke">#ffffff</CssParameter>
                            <CssParameter name="stroke-width">0.1</CssParameter>
                        </Stroke>
                    </PolygonSymbolizer>
                </Rule>
                <Rule>
                    <Name>Label</Name>
                    <MaxScaleDenominator>40000</MaxScaleDenominator>
                    <TextSymbolizer>
                        <Label>
                            <ogc:PropertyName>ParcelNumber</ogc:PropertyName>
                        </Label>
                        <Font>
                            <CssParameter name="font-family">Arial</CssParameter>
                            <CssParameter name="font-size">10</CssParameter>
                            <CssParameter name="font-weight">bold</CssParameter>
                            <CssParameter name="font-style">italic</CssParameter>
                        </Font>
                        <LabelPlacement>
                            <PointPlacement>
                                <AnchorPoint>
                                    <AnchorPointX>0.5</AnchorPointX>
                                    <AnchorPointY>0.5</AnchorPointY>
                                </AnchorPoint>
                            </PointPlacement>
                        </LabelPlacement>
                        <Fill>
                            <CssParameter name="fill">#ffffff</CssParameter>
                        </Fill>
                        <VendorOption name="autoWrap">60</VendorOption>
                        <VendorOption name="maxDisplacement">150</VendorOption>
                        <VendorOption name="repeat">-1</VendorOption>
                        <VendorOption name="partials">true</VendorOption>
                    </TextSymbolizer>
                </Rule>
            </FeatureTypeStyle>
        </UserStyle>
    </NamedLayer>
</StyledLayerDescriptor>
