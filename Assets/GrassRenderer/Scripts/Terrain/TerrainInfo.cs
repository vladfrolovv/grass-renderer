using UnityEngine;
namespace GrassRenderer.Terrain
{
    public struct TerrainInfo
    {

        public TerrainInfo(Sprite heightMap, int heightScale, int terrainSize, int terrainScale)
        {
            HeightMap = heightMap;
            HeightScale = heightScale;
            TerrainSize = terrainSize;
            TerrainScale = terrainScale;
        }

        public Sprite HeightMap { get; }
        public int HeightScale { get; }
        public int TerrainSize { get; }
        public int TerrainScale { get; }

    }
}
