using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Jobs;
namespace GrassRenderer.MeshGrass
{
    [BurstCompile]
    public struct MeshGrassSwayingJob : IJobParallelForTransform
    {

        [ReadOnly] public float Time;
        [ReadOnly] public float Amplitude;
        [ReadOnly] public float3 WindDirection;

        public void Execute(int index, TransformAccess transform)
        {
            float sway = math.sin(Time) * Amplitude;
            transform.rotation = quaternion.Euler(sway * WindDirection.x, transform.rotation.y,
                sway * WindDirection.z);
        }

    }
}
