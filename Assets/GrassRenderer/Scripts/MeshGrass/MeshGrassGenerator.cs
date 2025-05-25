using System;
using System.Collections.Generic;
using GrassRenderer.Terrain;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;
namespace GrassRenderer.MeshGrass
{
    public class MeshGrassGenerator : GrassGenerator
    {

        [Header("Mesh Grass Info")]
        [SerializeField] private GameObject _grassPrefab;
        [SerializeField] private Transform _grassParent;
        [SerializeField] private MeshGrassInfo _meshGrassInfo;

        [Header("Terrain References")]
        [SerializeField] private TerrainMeshGenerator _terrainMeshGenerator;

        private readonly List<Transform> _grassTransforms = new();
        private readonly Subject<IEnumerable<Transform>> _onGrassTransformsUpdate = new();

        public IObservable<IEnumerable<Transform>> OnGrassTransformsUpdate => _onGrassTransformsUpdate;

        public override void GenerateGrass(TerrainInfo terrainInfo)
        {
            ClearGrass();
            int grassCount = Mathf.RoundToInt(_terrainMeshGenerator.TerrainSize / _meshGrassInfo.BoxSize);
            for (int z = 0; z < grassCount; z++)
            {
                for (int x = 0; x < grassCount; x++)
                {
                    GameObject grassObject = Instantiate(_grassPrefab, _grassParent);
                    Vector3 position = new (
                        x * _meshGrassInfo.BoxSize - _terrainMeshGenerator.TerrainSize / 2f + _meshGrassInfo.BoxSize / 2f, 0,
                        z * _meshGrassInfo.BoxSize + _meshGrassInfo.BoxSize / 2f);

                    position += new Vector3(
                        Random.Range(-_meshGrassInfo.BoxSize / 2f, _meshGrassInfo.BoxSize / 2f), 0,
                        Random.Range(-_meshGrassInfo.BoxSize / 2f, _meshGrassInfo.BoxSize / 2f));

                    grassObject.transform.position = position;
                    grassObject.transform.localRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                    _grassTransforms.Add(grassObject.transform);
                }
            }

            _onGrassTransformsUpdate.OnNext(_grassTransforms);
        }

        public void ClearGrass()
        {
            while (_grassParent.childCount > 0) {
                DestroyImmediate(_grassParent.GetChild(0).gameObject);
            }

            _grassTransforms.Clear();
            _onGrassTransformsUpdate.OnNext(_grassTransforms);
        }

    }
}
