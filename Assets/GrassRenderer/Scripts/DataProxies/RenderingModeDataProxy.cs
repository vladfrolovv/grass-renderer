namespace GrassRenderer.DataProxies
{
    public class RenderingModeDataProxy
    {

        public bool UseRenderingModeSettings { get; private set; }

        public int TerrainSize { get; private set; } = 32;
        public bool UseHeightMap { get; private set; } = false;
        public int HeightScale { get; private set; } = 0;

        public void SetTerrainSize(int terrainSize)
        {
            TerrainSize = terrainSize;
            UseRenderingModeSettings = true;
        }

        public void SetUseHeightMap(bool useHeightMap)
        {
            UseHeightMap = useHeightMap;
            UseRenderingModeSettings = true;

            if (UseHeightMap)
            {
                HeightScale++;
            }
        }

    }
}
