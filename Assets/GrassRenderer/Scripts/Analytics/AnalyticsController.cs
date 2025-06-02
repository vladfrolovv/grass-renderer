using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using Object = UnityEngine.Object;
namespace GrassRenderer.Analytics
{
    public class AnalyticsController : IAnalyticsController
    {

        private readonly ReactiveProperty<AnalyticsFrameModel> _frameData = new();

        private readonly List<MeshFilter> _meshFilters = new();
        private readonly Camera _targetCamera;
        private Plane[] _frustumPlanes;

        public IReadOnlyReactiveProperty<AnalyticsFrameModel> FrameData => _frameData;

        public AnalyticsController(Camera targetCamera = null)
        {
            _targetCamera = targetCamera;
            RefreshMeshCache();

            Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ => Tick());
        }

        public void RefreshMeshCache()
        {
            _meshFilters.Clear();
            _meshFilters.AddRange(Object.FindObjectsOfType<MeshFilter>());
        }

        public void Tick()
        {
            AnalyticsFrameModel model = new()
            {
                DeltaTime = Time.unscaledDeltaTime,
                FPS = 1f / Time.unscaledDeltaTime
            };

            _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_targetCamera);

            GetVisibleVerticesAndTriangles(out model.VisibleVertices, out model.VisibleTriangles);
            GetGPUAndCPUFrameTime(out model.GPUFrameTimeMs, out model.CPUFrameTimeMs);

            _frameData.Value = model;
        }

        private void GetVisibleVerticesAndTriangles(out int visibleVertices, out int visibleTriangles)
        {
            visibleVertices = 0;
            visibleTriangles = 0;

            foreach (MeshFilter meshFilter in _meshFilters)
            {
                Renderer rend = meshFilter.GetComponent<Renderer>();
                if (!rend || !rend.enabled || !rend.gameObject.activeInHierarchy) continue;
                if (!GeometryUtility.TestPlanesAABB(_frustumPlanes, rend.bounds)) continue;

                Mesh mesh = meshFilter.sharedMesh;
                if (mesh == null) continue;

                visibleVertices += mesh.vertexCount;
                visibleTriangles += mesh.triangles.Length / 3;
            }
        }

        private void GetGPUAndCPUFrameTime(out float gpuFrameTime, out float cpuFrameTime)
        {
            FrameTimingManager.CaptureFrameTimings();
            FrameTiming[] ft = new FrameTiming[1];
            if (FrameTimingManager.GetLatestTimings(1, ft) > 0)
            {
                gpuFrameTime = (float)ft[0].gpuFrameTime;
                cpuFrameTime = (float)ft[0].cpuFrameTime;
            }
            else
            {
                gpuFrameTime = 0f;
                cpuFrameTime = 0f;
            }
        }

    }
}
