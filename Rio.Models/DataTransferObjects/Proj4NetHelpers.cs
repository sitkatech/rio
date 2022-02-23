using System;
using System.Collections.Generic;
using NetTopologySuite.Geometries;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

namespace Rio.Models.DataTransferObjects
{
    public static class Proj4NetHelpers
    {
        private static readonly Dictionary<int, string> CoordinateSystemsWkTs = new Dictionary<int, string>
        {
            [2229] = @"
PROJCS[""NAD83 / California zone 5 (ftUS)"",
    GEOGCS[""NAD83"",
        DATUM[""North_American_Datum_1983"",
            SPHEROID[""GRS 1980"",6378137,298.257222101,
                AUTHORITY[""EPSG"",""7019""]],
            TOWGS84[0,0,0,0,0,0,0],
            AUTHORITY[""EPSG"",""6269""]],
        PRIMEM[""Greenwich"",0,
            AUTHORITY[""EPSG"",""8901""]],
        UNIT[""degree"",0.0174532925199433,
            AUTHORITY[""EPSG"",""9122""]],
        AUTHORITY[""EPSG"",""4269""]],
    PROJECTION[""Lambert_Conformal_Conic_2SP""],
    PARAMETER[""standard_parallel_1"",35.46666666666667],
    PARAMETER[""standard_parallel_2"",34.03333333333333],
    PARAMETER[""latitude_of_origin"",33.5],
    PARAMETER[""central_meridian"",-118],
    PARAMETER[""false_easting"",6561666.667],
    PARAMETER[""false_northing"",1640416.667],
    UNIT[""US survey foot"",0.3048006096012192,
        AUTHORITY[""EPSG"",""9003""]],
    AXIS[""X"",EAST],
    AXIS[""Y"",NORTH],
    AUTHORITY[""EPSG"",""2229""]]
"
        };

        public static bool WkTHasAppropriateSRS(string wkt, int targetSrid)
        {
            return wkt
                .Replace("\r\n", string.Empty)
                .Replace("\t", string.Empty)
                .Replace(" ", string.Empty)
                .ToLower()
                .Trim()
                .Contains(CoordinateSystemsWkTs[targetSrid]
                    .Replace("\r\n", string.Empty)
                    .Replace("\t", string.Empty)
                    .Replace(" ", string.Empty)
                    .ToLower()
                    .Trim());
        }

        private static Geometry Transform(Geometry geom, MathTransform transform, int targetSrid)
        {
            geom = geom.Copy();
            geom.Apply(new MathTransformFilter(transform));
            geom.SRID = targetSrid;
            return geom;
        }

        public static Geometry ProjectTo4326(this Geometry geometry)
        {
            var sourceCoordinateSystem = new CoordinateSystemFactory().CreateFromWkt(CoordinateSystemsWkTs[geometry.SRID]);
            var transformation = new CoordinateTransformationFactory().CreateFromCoordinateSystems(sourceCoordinateSystem, GeographicCoordinateSystem.WGS84);
            return Transform(geometry, transformation.MathTransform, 4326);
        }
    }

    internal sealed class MathTransformFilter : ICoordinateSequenceFilter
    {
        private readonly MathTransform _mathTransform;

        public MathTransformFilter(MathTransform mathTransform) => _mathTransform = mathTransform;

        public bool Done => false;
        public bool GeometryChanged => true;
        public void Filter(CoordinateSequence seq, int i)
        {
            var x = seq.GetX(i);
            var y = seq.GetY(i);
            //var z = seq.GetZ(i);
            //_mathTransform.Transform(ref x, ref y, ref z);
            _mathTransform.Transform(ref x, ref y);
            seq.SetX(i, x);
            seq.SetY(i, y);
            //seq.SetZ(i, z);
        }
    }
}