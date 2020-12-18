using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Prepared;
using NetTopologySuite.Index;
using NetTopologySuite.Index.Strtree;
using NetTopologySuite.IO;
using NetTopologySuite.Operation.Buffer;
using Newtonsoft.Json;

namespace Rio.API.GeoSpatial
{
    public static class GeoJsonHelpers
    {
        public static FeatureCollection GetFeatureCollectionFromGeoJsonFile(string pathToGeoJsonFile,
            int coordinatePrecision)
        {
            FeatureCollection featureCollection;
            using (var streamReader = new StreamReader(File.OpenRead(pathToGeoJsonFile)))
            using (var jsonReader = new JsonTextReader(streamReader))
            {

                var scale = Math.Pow(10, coordinatePrecision);
                var geometryFactory = new GeometryFactory(new PrecisionModel(scale),
                    Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId);
                var reader = new GeoJsonReader(geometryFactory, new JsonSerializerSettings());
                featureCollection = reader.Read<FeatureCollection>(jsonReader);
            }

            return featureCollection;
        }

        public static FeatureCollection GetFeatureCollectionFromGeoJsonString(string geoJson,
            int coordinatePrecision)
        {
            var scale = Math.Pow(10, coordinatePrecision);
            var geometryFactory = new GeometryFactory(new PrecisionModel(scale),
                Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId);
            var reader = new GeoJsonReader(geometryFactory, new JsonSerializerSettings());
            var featureCollection = reader.Read<FeatureCollection>(geoJson);
            return featureCollection;
        }

        public static FeatureCollection GetFeatureCollectionFromGeoJsonByteArray(byte[] fileContentsByteArray,
            int coordinatePrecision)
        {
            FeatureCollection featureCollection;
            using (var streamReader = new StreamReader(new MemoryStream(fileContentsByteArray)))
            using (var jsonReader = new JsonTextReader(streamReader))
            {
                featureCollection = ExtractFeatureCollectionFromJson(coordinatePrecision, jsonReader);
            }

            return featureCollection;
        }

        public static FeatureCollection ExtractFeatureCollectionFromJson(int coordinatePrecision, JsonTextReader jsonReader)
        {
            var scale = Math.Pow(10, coordinatePrecision);
            var geometryFactory = new GeometryFactory(new PrecisionModel(scale),
                Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId);
            var reader = new GeoJsonReader(geometryFactory, new JsonSerializerSettings());
            var featureCollection = reader.Read<FeatureCollection>(jsonReader);
            return featureCollection;
        }

        public static string GetGeoJsonStringFromGeoJsonByteArray(byte[] fileContentsByteArray)
        {
            return Encoding.UTF8.GetString(fileContentsByteArray);
        }

        public static List<IFeature> GetFeatureCollectionFromGeoJsonByteArray(byte[] fileContentsByteArray,
            int coordinatePrecision, IPreparedGeometry boundingBox)
        {
            var featureCollection = GetFeatureCollectionFromGeoJsonByteArray(fileContentsByteArray, coordinatePrecision);
            return featureCollection.Where(x => boundingBox.Intersects(x.Geometry)).ToList();
        }

        public static Feature ToGeoJsonFeature<T>(this T featureClass) where T : IHasGeometry
        {
            var dictionary = PocoToDictionary.ToDictionary<object>(featureClass);
            var attributesTable = new AttributesTable(dictionary);
            var feature = new Feature(featureClass.Geometry, attributesTable);
            return feature;
        }

        public static void WriteFeaturesToGeoJsonFile(string fileOutput, IEnumerable<IFeature> features, int numberOfSignificantDigits)
        {
            var featureCollection = new FeatureCollection();
            foreach (var feature in features)
            {
                featureCollection.Add(feature);
            }
            WriteFeaturesToGeoJsonFile(fileOutput, featureCollection, numberOfSignificantDigits);
        }

        public static void WriteFeaturesToGeoJsonFile(string fileOutput, FeatureCollection featureCollection,
            int numberOfSignificantDigits)
        {
            var gjw = new GeoJsonWriter()
            {
                SerializerSettings =
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter> {new DoubleFormatConverter(numberOfSignificantDigits)}
                }
            };

            using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(fileOutput)))
            {
                gjw.Write(featureCollection, jsonTextWriter);
            }
        }

        public static MemoryStream WriteFeaturesToGeoJsonStream(IEnumerable<IFeature> features, int numberOfSignificantDigits)
        {
            var featureCollection = new FeatureCollection();
            foreach (var feature in features)
            {
                featureCollection.Add(feature);
            }

            return WriteFeaturesToGeoJsonStream(featureCollection, numberOfSignificantDigits);
        }

        public static MemoryStream WriteFeaturesToGeoJsonStream(FeatureCollection featureCollection,
            int numberOfSignificantDigits)
        {
            var stream = new MemoryStream();
            var gjw = new GeoJsonWriter()
            {
                SerializerSettings =
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter> {new DoubleFormatConverter(numberOfSignificantDigits)}
                }
            };
            using (var jsonTextWriter = new JsonTextWriter(new StreamWriter(stream)))
            {
                gjw.Write(featureCollection, jsonTextWriter);
            }

            return stream;
        }

        public static string WriteFeaturesToGeoJsonString(IEnumerable<IFeature> features,
            int numberOfSignificantDigits)
        {
            var featureCollection = new FeatureCollection();
            foreach (var feature in features)
            {
                featureCollection.Add(feature);
            }

            return WriteFeaturesToGeoJsonString(featureCollection, numberOfSignificantDigits);
        }

        public static string WriteFeaturesToGeoJsonString(FeatureCollection featureCollection,
            int numberOfSignificantDigits)
        {
            var sb = new StringBuilder();
            var gjw = new GeoJsonWriter()
            {
                SerializerSettings =
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    FloatParseHandling = FloatParseHandling.Decimal,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter> {new DoubleFormatConverter(numberOfSignificantDigits)}
                }
            };
            using (var jsonTextWriter = new JsonTextWriter(new StringWriter(sb)))
            {
                gjw.Write(featureCollection, jsonTextWriter);
            }

            return sb.ToString();
        }

        public static Geometry GetExtentForFeatureCollection(FeatureCollection featureCollection)
        {
            var maxX = featureCollection.Max(x => x.Geometry.EnvelopeInternal.MaxX);
            var minX = featureCollection.Min(x => x.Geometry.EnvelopeInternal.MinX);
            var maxY = featureCollection.Max(x => x.Geometry.EnvelopeInternal.MaxY);
            var minY = featureCollection.Min(x => x.Geometry.EnvelopeInternal.MinY);
            var wkt = $"POLYGON(({minX} {minY}, {minX} {maxY}, {maxX} {maxY}, {maxX} {minY}, {minX} {minY}))";
            var wktReader = new WKTReader();
            var boundingBox = wktReader.Read(wkt);
            boundingBox.SRID = Ogr2OgrCommandLineRunner.DefaultCoordinateSystemId;
            return boundingBox;
        }

        /// <summary>
        /// Given a linestring, a polygon feature class, returns the length of the linestring contained within a polygon in the feature class.
        /// </summary>
        /// <param name="lineGeometry"></param>
        /// <param name="polygonsToCheck"></param>
        /// <returns></returns>
        public static double CalculateLengthOfLineWithinPolygon(Geometry lineGeometry, IEnumerable<Geometry> polygonsToCheck)
        {
            var intersectingPolygons = polygonsToCheck.Where(x => x.Intersects(lineGeometry)).ToList();
            if (!intersectingPolygons.Any())
            {
                return 0;
            }

            var intersections = intersectingPolygons.Select(x => x.Intersection(lineGeometry)).Where(x => x.IsValid).ToList();
            return intersections.Any() ? intersections.Sum(x => x.Length) : 0;
        }

        public static double CalculateLengthOfLineWithinPolygon(Geometry lineGeometry, ISpatialIndex<Geometry> geometriesToCheck)
        {
            // restrict the search space to nodes in the bounding box of the buffered line--subquadratic speedup over a naive search
            var candidateGeometriesToCheck = geometriesToCheck.Query(lineGeometry.EnvelopeInternal);
            return CalculateLengthOfLineWithinPolygon(lineGeometry, candidateGeometriesToCheck);
        }

        /// <summary>
        /// Given a linestring, a polygon feature class, and a buffer distance (given in feet), returns the length of the linestring contained within the buffer distance of a polygon in the feature class.
        /// </summary>
        /// <param name="lineGeometry"></param>
        /// <param name="polygonsToCheck"></param>
        /// <param name="bufferDistance"></param>
        /// <returns></returns>
        public static double CalculateLengthOfLineWithinPolygon(Geometry lineGeometry, IEnumerable<Geometry> polygonsToCheck, int bufferDistance)
        {
            if (!(bufferDistance > 0))
            {
                return CalculateLengthOfLineWithinPolygon(lineGeometry, polygonsToCheck);
            }

            var bufferedLine = BufferOp.Buffer(lineGeometry, bufferDistance);
            var intersectingPolygon = polygonsToCheck.FirstOrDefault(x => x.Intersects(bufferedLine));

            if (intersectingPolygon == null) return 0;

            // buffer the geometry that intersects and calculate its intersection with the original line
            var bufferedIntersectingPolygon = BufferOp.Buffer(intersectingPolygon, bufferDistance);
            return bufferedIntersectingPolygon.Intersection(lineGeometry).Length;
        }

        public static IntersectionAndProximalCount CountProximalAndIntersectingGeometries(Geometry lineGeometry, ISpatialIndex<Geometry> geometriesToCheck, double bufferDistance, EndCapStyle bufferEndCapStyle)
        {
            var buffered = BufferOp.Buffer(lineGeometry, bufferDistance, new BufferParameters {EndCapStyle = bufferEndCapStyle});
            var candidateGeometriesToCheck = geometriesToCheck.Query(buffered.EnvelopeInternal);
            var proximals = candidateGeometriesToCheck.Where(x => x.Intersects(buffered)).ToList();
            var intersectionCount = proximals.Count(x => x.Intersects(lineGeometry));
            var proximalCount = proximals.Count - intersectionCount;
            return new IntersectionAndProximalCount { IntersectionCount = intersectionCount, ProximalCount = proximalCount };
        }

        public static IntersectionAndProximalCount CountProximalAndIntersectingGeometries(Geometry lineGeometry, STRtree<Geometry> geometriesToCheck, STRtree<Geometry> bufferedGeometriesToCheck)
        {
            var candidateProximalGeometriesToCheck = bufferedGeometriesToCheck.Query(lineGeometry.EnvelopeInternal);
            var candidateIntersectingGeometriesToCheck = geometriesToCheck.Query(lineGeometry.EnvelopeInternal);
            var proximalsAndIntersectionsCount = candidateProximalGeometriesToCheck.Count(x => x.Intersects(lineGeometry));
            var intersectionCount = candidateIntersectingGeometriesToCheck.Count(x => x.Intersects(lineGeometry));
            var proximalCount = proximalsAndIntersectionsCount - intersectionCount;
            return new IntersectionAndProximalCount { IntersectionCount = intersectionCount, ProximalCount = proximalCount };
        }

        public static int CountIntersectingGeometries(Geometry lineGeometry, ISpatialIndex<Geometry> geometriesToCheck)
        {
            var candidateGeometriesToCheck = geometriesToCheck.Query(lineGeometry.EnvelopeInternal);
            return candidateGeometriesToCheck.Count(x => x.Intersects(lineGeometry));
        }

        // special case for streets

        public static bool CheckForIntersection(Geometry geometry, STRtree<Geometry> possibleIntersections)
        {
            return possibleIntersections.Query(geometry.EnvelopeInternal).Any(x => x.Intersects(geometry));
        }
    }
}
