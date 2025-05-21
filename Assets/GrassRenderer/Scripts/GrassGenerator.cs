using GrassRenderer.Terrain;
using UnityEngine;
namespace GrassRenderer
{
    public abstract class GrassGenerator : MonoBehaviour
    {
        public abstract void GenerateGrass(TerrainInfo terrainInfo);
    }
}
