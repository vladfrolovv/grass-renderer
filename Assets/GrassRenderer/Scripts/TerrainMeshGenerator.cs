using System;
using GrassRenderer.Terrain;
using GrassRenderer.Utilities;
using UnityEngine;
namespace GrassRenderer
{
    public class TerrainMeshGenerator : MonoBehaviour
    {

        [Header("Mesh References")]
        [SerializeField] private MeshFilter _meshFilter;
        [SerializeField] private MeshRenderer _meshRenderer;

        [Header("Terrain Settings")]
        [ReadOnly, SerializeField]
        private int _seed;

        [Tooltip("If height map is not used, terrain takes only two triangles.\n" +
                 "If height map is used, terrain takes 2 * terrainSize * terrainSize triangles.")]
        [SerializeField] private bool _useHeightMap;

        [SerializeField, HideIf("_useHeightMap")] private Sprite _heightMapTexture;
        [SerializeField, HideIf("_useHeightMap")] private int _terrainHeight;

        [SerializeField, Range(0, 255)] private int _terrainSize;
        [SerializeField, Range(1, 100)] private int _sizeScale;

        [Header("Grass")]
        [SerializeField] private GrassGenerator _grassGenerator;

        private Mesh _terrainMesh;
        private Vector3[] _vertices;
        private int[] _triangles;

        public int TerrainSize => _terrainSize * _sizeScale;

        private void Awake()
        {
            GenerateNewTerrain();
        }

        public void GenerateNewTerrain()
        {
            _terrainMesh = new Mesh();
            if (_useHeightMap)
            {
                GeneratePlaneWithHeightMap();
            }
            else
            {
                GenerateQuad();
            }

            UpdateMesh();

            _grassGenerator
                .GenerateGrass(new TerrainInfo(_heightMapTexture, _terrainHeight, TerrainSize, _sizeScale));
        }

        public void ClearMesh()
        {
            _vertices = Array.Empty<Vector3>();
            _triangles = Array.Empty<int>();

            _terrainMesh.Clear();
            _meshFilter.mesh = null;
        }

        private void GenerateQuad()
        {
            float width = TerrainSize;
            float depth = TerrainSize;
            _vertices = new Vector3[]
            {
                new (0, 0, 0),
                new (width, 0, 0),
                new (0, 0, depth),
                new (width, 0, depth)
            };

            _triangles = new []
            {
                0, 2, 1,
                2, 3, 1
            };
        }

        private void GeneratePlaneWithHeightMap()
        {
            int size = _terrainSize;
            _vertices = new Vector3[(size + 1) * (size + 1)];
            _triangles = new int[size * size * 6];

            Texture2D tex = _heightMapTexture.texture;
            Rect rect = _heightMapTexture.textureRect;
            Color[] pixels = tex.GetPixels(
                (int)rect.x,
                (int)rect.y,
                (int)rect.width,
                (int)rect.height);

            int i = 0;
            for (int z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    int px = Mathf.Clamp(x * tex.width / size, 0, tex.width - 1);
                    int pz = Mathf.Clamp(z * tex.height / size, 0, tex.height - 1);
                    float heightNorm = pixels[pz * (int)rect.width + px].r;
                    float y = heightNorm * _terrainHeight;

                    _vertices[i++] = new Vector3(x * _sizeScale, y, z * _sizeScale);
                }
            }

            int vi = 0;
            int ti = 0;
            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    _triangles[ti    ] = vi;
                    _triangles[ti + 1] = vi + size + 1;
                    _triangles[ti + 2] = vi + 1;
                    _triangles[ti + 3] = vi + 1;
                    _triangles[ti + 4] = vi + size + 1;
                    _triangles[ti + 5] = vi + size + 2;

                    vi++;
                    ti += 6;
                }
                vi++;
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

            _meshRenderer.transform.position = new Vector3(-TerrainSize / 2f, 0, 0);
        }

    }
}