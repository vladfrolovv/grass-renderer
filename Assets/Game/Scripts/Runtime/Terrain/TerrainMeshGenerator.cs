using System.Collections.Generic;
using Core.Runtime.Attributes;
using Core.Runtime.Base;
using UnityEngine;
namespace Game.Scripts.Runtime.Terrain
{
    public class TerrainMeshGenerator : BaseBehaviour
    {

        [ReadOnly] private int _seed;

        [Header("References")]
        [SerializeField] private MeshFilter _meshFilter;

        [Header("Mesh Configuration")]
        [Range(1, 1000)]
        [SerializeField] private int _width;

        [Range(1, 1000)]
        [SerializeField] private int _length;

        [Range(1, 100)]
        [SerializeField] private float _scale;

        [Range(1, 100)]
        [SerializeField] private int _octaves;

        [Header("Height Configuration")]
        [SerializeField] private AnimationCurve _heightCurve;

        [Range(1, 100)]
        [SerializeField] private float _amplitude;

        [Range(1, 100)]
        [SerializeField] private float _frequency;

        [Range(0, 1)]
        [SerializeField] private float _persistence;

        [Range(1, 100)]
        [SerializeField] private float _lacunarity;

        [Range(-25, 25)]
        [SerializeField] private float _minHeight;

        [Range(-25, 25)]
        [SerializeField] private float _maxHeight;

        private Mesh _mesh;

        public void Generate()
        {
            _mesh = CreateMeshFrom(
                GenerateMeshVertices(),
                GenerateMeshTriangles());
        }

        public void Clear()
        {
            _meshFilter.mesh = null;
        }

        private Vector3[] GenerateMeshVertices()
        {
            Vector3[] vertices = new Vector3[(_width + 1) * (_length + 1)];
            int vertexIndex = 0;

            List<Vector2> offsets = CreteOffsetsList();

            for (int z = 0; z < _length; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float noiseHeight = ClampHeight(
                        CreateNoiseHeight(z, x, offsets));

                    vertices[vertexIndex] = (new Vector3(x, noiseHeight, z));
                    vertexIndex++;
                }
            }

            return vertices;
        }

        private int[] GenerateMeshTriangles()
        {
            int[] triangles = new int[_width * _length * 6];

            int verticesIndex = 0;
            int triangleIndex = 0;
            for (int z = 0; z < _length; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    triangles[triangleIndex + 0] = verticesIndex + 0;
                    triangles[triangleIndex + 1] = verticesIndex + _width + 1;
                    triangles[triangleIndex + 2] = verticesIndex + 1;
                    triangles[triangleIndex + 3] = verticesIndex + 1;
                    triangles[triangleIndex + 4] = verticesIndex + _width + 1;
                    triangles[triangleIndex + 5] = verticesIndex + _width + 2;

                    verticesIndex++;
                    triangleIndex += 6;
                }

                verticesIndex++;
            }

            return triangles;
        }

        private Mesh CreateMeshFrom(Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = triangles,
            };

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _meshFilter.sharedMesh = mesh;

            return mesh;
        }

        private List<Vector2> CreteOffsetsList()
        {
            _seed = Random.Range(0, 1000);

            System.Random random = new System.Random(_seed);
            List<Vector2> offsets = new List<Vector2>();

            for (int i = 0; i < _octaves; i++)
            {
                offsets.Add(new Vector2(
                    random.Next(-10000, 10000),
                    random.Next(-10000, 10000)));
            }

            return offsets;
        }

        private float CreateNoiseHeight(int z, int x, List<Vector2> offsets)
        {
            float frequency = _frequency;
            float amplitude = _amplitude;
            float noiseHeight = 0;

            for (int i = 0; i < _octaves; i++)
            {
                float xCoordinate = x / _scale * frequency + offsets[i].x;
                float zCoordinate = z / _scale * frequency + offsets[i].y;

                float perlinValue = Mathf.PerlinNoise(zCoordinate, xCoordinate) * 2 - 1;
                noiseHeight += _heightCurve.Evaluate(perlinValue) * amplitude;

                frequency *= _lacunarity;
                amplitude *= _persistence;
            }

            return noiseHeight;
        }

        private float ClampHeight(float height)
        {
            return Mathf.Clamp(height, _minHeight, _maxHeight);
        }

    }
}
