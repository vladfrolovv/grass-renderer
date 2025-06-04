using GrassRenderer.Analytics;
using GrassRenderer.DataProxies;
using GrassRenderer.Terrain;
using UnityEngine;
using Zenject;
namespace GrassRenderer
{
    public abstract class GrassGenerator : MonoBehaviour
    {
        public abstract void GenerateGrass(TerrainInfo terrainInfo);

        protected AnalyticsController AnalyticsController;

        [Inject]
        public void Construct(AnalyticsController analyticsController)
        {
            AnalyticsController = analyticsController;
        }

    }
}
