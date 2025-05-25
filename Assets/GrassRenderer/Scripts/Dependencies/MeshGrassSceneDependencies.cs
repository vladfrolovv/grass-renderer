using GrassRenderer.MeshGrass;
using UnityEngine;
using Zenject;
namespace GrassRenderer.Dependencies
{
    public class MeshGrassSceneDependencies : MonoInstaller
    {

        [SerializeField] private MeshGrassGenerator _meshGrassGenerator;

        public override void InstallBindings()
        {
            Container.BindInstance(_meshGrassGenerator).AsSingle();
            Container.BindInterfacesAndSelfTo<MeshGrassSwayingController>().AsSingle();
        }

    }
}
