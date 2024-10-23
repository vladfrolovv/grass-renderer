using System.Collections.Generic;
using Core.Runtime.Attributes;
using Core.Runtime.Base;
using UnityEngine;
namespace Game.Scripts.Runtime.Terrain
{
    public class TerrainMeshGenerator : BaseBehaviour
    {

        [Header("General")]
        [ReadOnly]
        [SerializeField] private int _seed = 0;

        [Header("References")]
        [SerializeField] private MeshFilter _meshFilter;

        [Header("Mesh Configuration")]
        [Range(1, 256)]
        [SerializeField] private int _width;

        [Range(1, 256)]
        [SerializeField] private int _length;

        private Mesh _mesh;

        public void Generate()
        {
            _mesh = CreateMeshFrom(
                GenerateMeshVertices(),
                GenerateMeshTriangles());
        }

        public void Clear()
        {
            _mesh = null;
            _meshFilter.mesh = null;
        }

        private Vector3[] GenerateMeshVertices()
        {
            List<Vector3> vertices = new List<Vector3>();
            for (int z = 0; z <= _length; z++)
            {
                for (int x = 0; x <= _width; x++)
                {
                    vertices.Add(new Vector3(x, 0f, z));
                }
            }

            return vertices.ToArray();
        }

        private int[] GenerateMeshTriangles()
        {
            List<int> triangles = new List<int>();

            int vertexIndex = 0;
            for (int z = 0; z < _length; z++)
            {
                for (int x = 0; x < _width; x++)
                {
                    triangles.Add(vertexIndex);
                    triangles.Add(vertexIndex + _width + 1);
                    triangles.Add(vertexIndex + 1);

                    triangles.Add(vertexIndex + 1);
                    triangles.Add(vertexIndex + _width + 1);
                    triangles.Add(vertexIndex + _width + 2);

                    vertexIndex++;
                }

                vertexIndex++;
            }


            return triangles.ToArray();
        }

        private Mesh CreateMeshFrom(Vector3[] vertices, int[] triangles)
        {
            Mesh mesh = new Mesh();

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;

            mesh.RecalculateNormals();
            mesh.RecalculateTangents();

            _meshFilter.sharedMesh = mesh;

            return mesh;
        }

    }
}
