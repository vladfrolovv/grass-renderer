using GrassRenderer.Analytics;
using UnityEngine;
using Zenject;
namespace GrassRenderer.Dependencies
{
    public class BillboardGrassSceneInstaller : MonoInstaller
    {

        [SerializeField] private Camera _camera;

        public override void InstallBindings()
        {
            Container.BindInstance(_camera).AsSingle();

            Container.BindInterfacesAndSelfTo<AnalyticsController>().AsSingle();
        }

    }
}
