namespace Rio.API.GeoSpatial
{
    public class IntersectionAndProximalCount
    {
        public int IntersectionCount { get; set; }
        public int ProximalCount { get; set; }
    }

    public class WaterMainXP : IntersectionAndProximalCount
    {
        public double? NearbyWaterMainMaxDiam { get; set; }
    }
}