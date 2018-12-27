using BruTile;
using BruTile.Predefined;
using BruTile.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BruTileTerrain
{
    class Program
    {
        static void Main(string[] args)
        {
            // input variablen: zoomLevel and Extent (in WGS94)
            // Sample: Amsterdam on level 12
            int zoomLevel = 12;
            var extent = new Extent(4.785233, 52.311837, 4.996548, 52.405142); //Amsterdam, Netherlands
            
            // Request OSM tiles
            var osmSchema = new GlobalSphericalMercator(0, 18);
            var osmTileSource = new HttpTileSource(osmSchema, "http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", new[] { "a", "b", "c" }, "OSM");
            var tilesOsm = GetOSMTiles(osmTileSource, extent, zoomLevel);
            PrintTiles("OSM", osmTileSource, tilesOsm);

            // Request terrain tiles
            var terrainUrl = @"https://maps.tilehosting.com/data/terrain-quantized-mesh/{z}/{x}/{y}.terrain?key=irAs6FzTF3gJ9ArfQPjh"; //max zoom is 13!       
            var globalGeodeticSchema = new TmsGlobalGeodeticTileSchema();
            var terrainTileSource = new HttpTileSource(globalGeodeticSchema, terrainUrl);
            var tilesTerrain = GetTiles(terrainTileSource, zoomLevel, extent);
            PrintTiles("Terrain", terrainTileSource, tilesTerrain);

            Console.ReadKey();
        }

        private static void PrintTiles(string message, HttpTileSource tileSource, List<TileInfo> tiles)
        {
            Console.WriteLine("Print tile from: " + message);
            foreach (var tileInfo in tiles)
            {
                var tile = tileSource.GetTile(tileInfo);

                Console.WriteLine(
                    $"tile col: {tileInfo.Index.Col}, " +
                    $"tile row: {tileInfo.Index.Row}, " +
                    $"tile level: {tileInfo.Index.Level} , " +
                    $"tile size {tile.Length}");

                // tile is a sort of image, todo: use stream in UNity3D
                var ms = new MemoryStream(tile);
            }
        }

        private static List<TileInfo> GetOSMTiles(HttpTileSource tileSource, Extent extent, int zoomLevel)
        {
            var from = SpatialConvertor.ToSphericalMercatorFromWgs84(extent.MinX, extent.MinY);
            var to = SpatialConvertor.ToSphericalMercatorFromWgs84(extent.MaxX, extent.MaxY);
            var extentSphericalMercator = new Extent(from[0], from[1], to[0], to[1]);

            var tiles = GetTiles(tileSource, zoomLevel, extentSphericalMercator);
            return tiles;
        }

        private static List<TileInfo> GetTiles(HttpTileSource tileSource, int zoomLevel, Extent extent)
        {
            var tileRange = TileTransform.WorldToTile(extent, zoomLevel.ToString(), tileSource.Schema);
            var tiles = tileSource.Schema.GetTileInfos(extent, zoomLevel.ToString()).ToList();
            return tiles;
        }
    }
}
