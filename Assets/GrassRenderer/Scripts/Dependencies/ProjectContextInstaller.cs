using GrassRenderer.DataProxies;
using Zenject;
namespace GrassRenderer.Dependencies
{
    public class ProjectContextInstaller : MonoInstaller
    {

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RenderingModeDataProxy>().AsSingle().NonLazy();
        }

    }
}
