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

            if (_targetCamera != null)
            {
                _frustumPlanes = GeometryUtility.CalculateFrustumPlanes(_targetCamera);

                int tris = 0, verts = 0;
                _meshFilters.ForEach(meshFilter =>
                {
                    Renderer rend = meshFilter.GetComponent<Renderer>();
                    if (!rend || !rend.enabled || !rend.gameObject.activeInHierarchy) return;
                    if (!GeometryUtility.TestPlanesAABB(_frustumPlanes, rend.bounds)) return;

                    Mesh mesh = meshFilter.sharedMesh;
                    if (mesh == null) return;

                    verts += mesh.vertexCount;
                    tris  += mesh.triangles.Length / 3;
                });

                model.VisibleVertices  = verts;
                model.VisibleTriangles = tris;
            }

            FrameTimingManager.CaptureFrameTimings();
            FrameTiming[] ft = new FrameTiming[1];
            if (FrameTimingManager.GetLatestTimings(1, ft) > 0)
            {
                model.CPUFrameTimeMs = (float)ft[0].cpuFrameTime;
                model.GPUFrameTimeMs = (float)ft[0].gpuFrameTime;
            }

            _frameData.Value = model;
        }
    }
}
