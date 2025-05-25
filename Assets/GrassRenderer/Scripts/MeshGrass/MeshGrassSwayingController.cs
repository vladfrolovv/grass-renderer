using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;
namespace GrassRenderer.MeshGrass
{
    public class MeshGrassSwayingController : IDisposable
    {

        private const float Amplitude = 0.1f;
        private readonly Vector3 _windDirection = new (1, 0, 1);

        private readonly CompositeDisposable _compositeDisposable = new();

        private TransformAccessArray _grassTransforms = new();

        public MeshGrassSwayingController(MeshGrassGenerator meshGrassGenerator)
        {
            Observable.EveryUpdate().Subscribe(OnGrassUpdate).AddTo(_compositeDisposable);
            meshGrassGenerator.OnGrassTransformsUpdate.Subscribe(UpdateTransformAccessArray).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        private void OnGrassUpdate(long l)
        {
            if (!_grassTransforms.isCreated) return;
            MeshGrassSwayingJob meshGrassSwayingJob = new()
            {
                Time = Time.time,
                Amplitude = Amplitude,
                WindDirection = _windDirection,
            };

            JobHandle swayingJobHandle = meshGrassSwayingJob.Schedule(_grassTransforms);
            swayingJobHandle.Complete();
        }

        private void UpdateTransformAccessArray(IEnumerable<Transform> transforms)
        {
            _grassTransforms = new TransformAccessArray(transforms.ToArray());
        }

    }
}
