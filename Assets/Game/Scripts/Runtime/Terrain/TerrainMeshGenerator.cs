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
        [Range(1, 100)]
        [SerializeField] private int _width;

        [Range(1, 100)]
        [SerializeField] private int _height;

        [Range(1, 100)]
        [SerializeField] private float _scale;

        [Range(1, 100)]
        [SerializeField] private int _octaves;

        private Mesh _mesh;

        public void Generate()
        {
            _mesh = new Mesh();
            _meshFilter.mesh = _mesh;

            UpdateMesh(
                GenerateMeshVertices(),
                GenerateMeshTriangles());
        }

        public void Clear()
        {
            _meshFilter.mesh = null;
        }

        private Vector3[] GenerateMeshVertices()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> offsets = CreteOffsetsList();

            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    float noiseHeight = Random.Range(-1, 1); // todo implement noise noise
                    vertices.Add(new Vector3(x, noiseHeight, y));
                }
            }

            return vertices.ToArray();
        }

        private int[] GenerateMeshTriangles()
        {
            List<int> triangles = new List<int>();

            int verticesIndex = 0;
            for (int y = 0; y < _height - 1; y++)
            {
                for (int x = 0; x < _width - 1; x++)
                {
                    triangles.Add(verticesIndex);
                    triangles.Add(verticesIndex + _width);
                    triangles.Add(verticesIndex + _width + 1);

                    triangles.Add(verticesIndex);
                    triangles.Add(verticesIndex + _width + 1);
                    triangles.Add(verticesIndex + 1);

                    verticesIndex++;
                }

                verticesIndex++;
            }

            return triangles.ToArray();
        }

        private void UpdateMesh(Vector3[] vertices, int[] triangles)
        {
            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.triangles = triangles;

            _mesh.RecalculateNormals();
            _mesh.RecalculateTangents();

            _meshFilter.sharedMesh = _mesh;
        }

        private List<Vector2> CreteOffsetsList()
        {
            List<Vector2> offsets = new List<Vector2>();

            for (int i = 0; i < _octaves; i++)
            {
                offsets.Add(new Vector2(
                    Random.Range(-10000, 10000),
                    Random.Range(-10000, 10000)));
            }

            return offsets;
        }

    }
}
