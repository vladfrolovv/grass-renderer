using UniRx;
namespace GrassRenderer.Analytics
{
    public interface IAnalyticsController
    {

        IReadOnlyReactiveProperty<AnalyticsFrameModel> FrameData { get; }

        void Tick();
        void RefreshMeshCache();

    }
}
