using UnityEngine;
namespace GrassRenderer.BillboardGrass
{
    public struct GrassData
    {

        public GrassData(Vector4 position, Vector2 uv, float displacement)
        {
            Position = position;
            UV = uv;
            Displacement = displacement;
        }

        public Vector4 Position { get; }
        public Vector2 UV { get; }
        public float Displacement { get; }

    }
}
