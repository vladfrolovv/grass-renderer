using GrassRenderer.Analytics;
using GrassRenderer.MeshGrass;
using UnityEngine;
using Zenject;
namespace GrassRenderer.Dependencies
{
    public class MeshGrassSceneInstaller : MonoInstaller
    {

        [SerializeField] private Camera _camera;
        [SerializeField] private MeshGrassGenerator _meshGrassGenerator;

        public override void InstallBindings()
        {
            Container.BindInstance(_camera).AsSingle();
            Container.BindInstance(_meshGrassGenerator).AsSingle();
            Container.BindInterfacesAndSelfTo<MeshGrassSwayingController>().AsSingle();

            Container.BindInterfacesAndSelfTo<AnalyticsController>().AsSingle();
        }

    }
}
