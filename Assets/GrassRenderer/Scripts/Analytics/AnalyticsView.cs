using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
namespace GrassRenderer.Analytics
{
    public class AnalyticsView : MonoBehaviour
    {

        [Header("View")]
        [SerializeField] private TextMeshProUGUI _deltaTimeText;
        [SerializeField] private TextMeshProUGUI _fpsText;
        [SerializeField] private TextMeshProUGUI _visibleTrianglesText;
        [SerializeField] private TextMeshProUGUI _visibleVerticesText;
        [SerializeField] private TextMeshProUGUI _cpuFrameTime;
        [SerializeField] private TextMeshProUGUI _gpuFrameTime;

        private IAnalyticsController _analyticsController;

        [Inject]
        public void Construct(IAnalyticsController analyticsController)
        {
            _analyticsController = analyticsController;
        }

        private void Awake()
        {
            _analyticsController.FrameData.Subscribe(UpdateFrameDataView);
        }

        private void UpdateFrameDataView(AnalyticsFrameModel analyticsFrameModel)
        {
            _deltaTimeText.text = $"Delta Time: {analyticsFrameModel.DeltaTime:F3}s";
            _fpsText.text = $"FPS: {analyticsFrameModel.FPS:F1}";
            _visibleTrianglesText.text = $"Visible Triangles: {analyticsFrameModel.VisibleTriangles}";
            _visibleVerticesText.text = $"Visible Vertices: {analyticsFrameModel.VisibleVertices}";
            _cpuFrameTime.text = $"CPU Frame Time: {analyticsFrameModel.CPUFrameTimeMs:F3}ms";
            _gpuFrameTime.text = $"GPU Frame Time: {analyticsFrameModel.GPUFrameTimeMs:F3}ms";
        }

    }
}
