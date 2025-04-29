using System;
using GrassRenderer.Utilities;
using UnityEngine;
namespace GrassRenderer.Terrain
{
    public class TerrainMeshGenerator : MonoBehaviour
    {

        [Header("Mesh References")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        [Header("Terrain Settings")]
        [ReadOnly, SerializeField]
        private int _seed;

        [Range(0, 255)]
        [SerializeField] private int _terrainSize;

        [Range(1, 100)]
        [SerializeField] private int _sizeScale;

        [Range(0, 100), Tooltip("The height of the terrain in percents.")]
        [SerializeField] private int _terrainHeight;

        private Mesh _terrainMesh;
        private Vector3[] _vertices;
        private int[] _triangles;

        public int TerrainSize => _terrainSize * _sizeScale;

        public void GenerateNewTerrain()
        {
            _terrainMesh = new Mesh();

            GeneratePlane();
            UpdateMesh();
        }

        public void ClearMesh()
        {
            _vertices = Array.Empty<Vector3>();
            _triangles = Array.Empty<int>();

            _terrainMesh.Clear();
            _meshFilter.mesh = null;
        }

        private void GeneratePlane()
        {
            _vertices = new Vector3[(_terrainSize + 1) * (_terrainSize + 1)];
            _triangles = new int[_terrainSize * _terrainSize * 6];

            for (int i = 0, z = 0; z <= _terrainSize; z++)
            {
                for (int x = 0; x <= _terrainSize; x++)
                {
                    _vertices[i] = new Vector3(x * _sizeScale, 0, z * _sizeScale);
                    i++;
                }
            }

            int vertexIndex = 0;
            int triangleIndex = 0;
            for (int z = 0; z < _terrainSize; z++)
            {
                for (int x = 0; x < _terrainSize; x++)
                {
                    _triangles[triangleIndex] = vertexIndex;
                    _triangles[triangleIndex + 1] = vertexIndex + _terrainSize + 1;
                    _triangles[triangleIndex + 2] = vertexIndex + 1;
                    _triangles[triangleIndex + 3] = vertexIndex + 1;
                    _triangles[triangleIndex + 4] = vertexIndex + _terrainSize + 1;
                    _triangles[triangleIndex + 5] = vertexIndex + _terrainSize + 2;

                    vertexIndex++;
                    triangleIndex += 6;
                }

                vertexIndex++;
            }

        }

        private void UpdateMesh()
        {
            _terrainMesh.Clear();
            _terrainMesh.vertices = _vertices;
            _terrainMesh.triangles = _triangles;

            _terrainMesh.RecalculateNormals();
            _terrainMesh.RecalculateBounds();

            _meshFilter.mesh = _terrainMesh;

            _meshRenderer.transform.position = new Vector3(-(float)_terrainSize * _sizeScale / 2, 0, 0);
        }

    }
}