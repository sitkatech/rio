using System.Collections.Generic;
using System.Linq;
using GeoAPI.Geometries;
using NetTopologySuite.Geometries;

namespace Rio.Models.DataTransferObjects
{
    public class BoundingBoxDto
    {
        public double Left;
        public double Bottom;
        public double Right;
        public double Top;

        public BoundingBoxDto(IReadOnlyCollection<Point> pointList)
        {
            if (pointList.Any())
            {
                Left = pointList.Min(x => x.X);
                Right = pointList.Max(x => x.X);
                Bottom = pointList.Min(x => x.Y);
                Top = pointList.Max(x => x.Y);
            }
            else
            {
                Left = -119.11015104115182;
                Top = 35.442022035628575;
                Right = -119.45272037350193;
                Bottom = 35.27608156273151;
            }
        }


        public BoundingBoxDto(IEnumerable<IGeometry> geometries) : this(geometries.SelectMany(BoundingBoxDto.GetPointsFromDbGeometry).ToList())
        {
        }

        public static List<Point> GetPointsFromDbGeometry(IGeometry geometry)
        {
            var pointList = new List<Point>();
            var envelope = geometry.EnvelopeInternal;
            pointList.Add(new Point(envelope.MinX, envelope.MinY));
            pointList.Add(new Point(envelope.MaxX, envelope.MaxY));
            return pointList;
        }
    }
}