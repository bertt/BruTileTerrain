using DotSpatial.Projections;

namespace BruTileTerrain
{
    public static class SpatialConvertor
    {
        public static double[] ToSphericalMercatorFromWgs84(double Longitude, double Latitude)
        {
            var src = ProjectionInfo.FromEpsgCode(4326);
            var target = ProjectionInfo.FromEpsgCode(3857);
            var points = new double[] { Longitude, Latitude };
            var z = new double[] { 0 };

            Reproject.ReprojectPoints(points, z, src, target, 0, 1);
            var result = new double[] { points[0], points[1] };
            return result;
        }
    }
}
