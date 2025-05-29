using GrassRenderer.Terrain;
using GrassRenderer.Utilities;
using UniRx;
using UnityEngine;
namespace GrassRenderer.BillboardGrass
{
    public class BillboardGrassGenerator : GrassGenerator
    {

        private const int GrassDataStride = 48;

        private const int IndirectArgsCount = 5;
        private const int IndirectArgsStride = sizeof(uint) * IndirectArgsCount;

        [Header("Grass Settings")]
        [SerializeField] private Vector3 _quadSize = new (500f, 500f, 500f);
        [SerializeField] private float _grassScale = 1.5f;
        [SerializeField] private float _noiseScale = 0.1f;
        [SerializeField] private Texture _heightMap;

        [Header("Shape")]
        [SerializeField] private Mesh _grassMesh;

        #region Grass Shader Properties

        [Header("Shader")]
        [SerializeField] private bool _showGrassShaderProperties = true;
        [SerializeField] private Material _grassMaterial;
        [SerializeField, HideIf("_showGrassShaderProperties")]
        private string _positionBufferPropertyName = "_PositionBuffer";

        [SerializeField, HideIf("_showGrassShaderProperties")]
        private string _rotationPropertyName = "_Rotation";

        [SerializeField, HideIf("_showGrassShaderProperties")]
        private string _grassQuadScalePropertyName = "_QuadScale";

        #endregion

        #region Compute Shader Properties

        [Header("Grass Compute Shader")]
        [SerializeField] private bool _showGrassComputeShaderProperties = true;
        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _grassComputeShaderPath = "ComputeShaders/GrassPositionsCalculator";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _dimensionsPropertyName = "_Dimension";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _scalePropertyName = "_Scale";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _displacementStrengthPropertyName = "_DisplacementStrength";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _noiseScalePropertyName = "_NoiseScale";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _heightMapPropertyName = "_HeightMap";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _yOffsetPropertyName = "_YOffset";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _grassDataBufferPropertyName = "_GrassDataBuffer";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _grassMaterialPropertyName = "_GrassMaterial";

        [SerializeField, HideIf("_showGrassComputeShaderProperties")]
        private string _grassMeshPropertyName = "_GrassMesh";

        #endregion

        private ComputeShader _grassInitializationShader;
        private ComputeShader _windComputeShader;

        private ComputeBuffer _grassDataBuffer;
        private ComputeBuffer _argumentsBuffer;

        private Material _grassMaterial1;
        private Material _grassMaterial2;
        private Material _grassMaterial3;

        private TerrainInfo _terrainInfo;

        public override void GenerateGrass(TerrainInfo terrainInfo)
        {
            _terrainInfo = terrainInfo;
            int resolution = _terrainInfo.TerrainSize * _terrainInfo.TerrainScale;

            _grassInitializationShader = Resources.Load<ComputeShader>(_grassComputeShaderPath);

            _grassDataBuffer = new ComputeBuffer(resolution * resolution, GrassDataStride);
            _argumentsBuffer = new ComputeBuffer(1, IndirectArgsStride, ComputeBufferType.IndirectArguments);

            UpdateGrassBuffer();
            Observable.EveryUpdate().Subscribe(_ => OnGrassUpdate()).AddTo(this);
        }

        private void UpdateGrassBuffer()
        {
            UpgradeGrassInitialization();
            UpgradeGrassArguments();
            UpgradeGrassMaterials();

            AnalyticsController.RefreshMeshCache();
        }

        private void UpgradeGrassInitialization()
        {
            int resolution = _terrainInfo.TerrainSize * _terrainInfo.TerrainScale;
            _grassInitializationShader.SetInt(_dimensionsPropertyName, resolution);
            _grassInitializationShader.SetInt(_scalePropertyName, _terrainInfo.TerrainScale);
            _grassInitializationShader.SetBuffer(0, _grassDataBufferPropertyName, _grassDataBuffer);
            _grassInitializationShader.SetTexture(0, _heightMapPropertyName, _heightMap);
            _grassInitializationShader.SetFloat(_displacementStrengthPropertyName, _terrainInfo.HeightScale);
            _grassInitializationShader.SetFloat(_noiseScalePropertyName, _noiseScale);
            _grassInitializationShader.SetFloat(_yOffsetPropertyName, _grassScale / 2f);
            _grassInitializationShader.SetFloat(_noiseScalePropertyName, _noiseScale);
            _grassInitializationShader.Dispatch(0,
                Mathf.CeilToInt(resolution / 8f),
                Mathf.CeilToInt(resolution / 8f), 1);
        }

        private void UpgradeGrassArguments()
        {
            uint[] args = new uint[5] { 0, 0, 0, 0, 0 };
            args[0] = (uint)_grassMesh.GetIndexCount(0);
            args[1] = (uint)_grassDataBuffer.count;
            args[2] = (uint)_grassMesh.GetIndexStart(0);
            args[3] = (uint)_grassMesh.GetBaseVertex(0);

            _argumentsBuffer.SetData(args);
        }

        private void UpgradeGrassMaterials()
        {
            _grassMaterial1 = new Material(_grassMaterial);
            _grassMaterial1.SetBuffer(_positionBufferPropertyName, _grassDataBuffer);
            _grassMaterial1.SetFloat(_rotationPropertyName, 0.0f);
            _grassMaterial1.SetFloat(_displacementStrengthPropertyName, _terrainInfo.HeightScale);
            _grassMaterial1.SetFloat(_grassQuadScalePropertyName, _grassScale);

            _grassMaterial2 = new Material(_grassMaterial);
            _grassMaterial2.SetBuffer(_positionBufferPropertyName, _grassDataBuffer);
            _grassMaterial2.SetFloat(_rotationPropertyName, 50.0f);
            _grassMaterial2.SetFloat(_displacementStrengthPropertyName, _terrainInfo.HeightScale);
            _grassMaterial2.SetFloat(_grassQuadScalePropertyName, _grassScale);

            _grassMaterial3 = new Material(_grassMaterial);
            _grassMaterial3.SetBuffer(_positionBufferPropertyName, _grassDataBuffer);
            _grassMaterial3.SetFloat(_rotationPropertyName, -50.0f);
            _grassMaterial3.SetFloat(_displacementStrengthPropertyName, _terrainInfo.HeightScale);
            _grassMaterial3.SetFloat(_grassQuadScalePropertyName, _grassScale);
        }

        private void OnGrassUpdate()
        {
            UpgradeGrassMaterials();
            Graphics.DrawMeshInstancedIndirect(_grassMesh, 0, _grassMaterial1,
                new Bounds(Vector3.zero, _quadSize), _argumentsBuffer);
            Graphics.DrawMeshInstancedIndirect(_grassMesh, 0, _grassMaterial2,
                new Bounds(Vector3.zero, _quadSize), _argumentsBuffer);
            Graphics.DrawMeshInstancedIndirect(_grassMesh, 0, _grassMaterial3,
                new Bounds(Vector3.zero, _quadSize), _argumentsBuffer);
        }

        private void OnDestroy()
        {
            _grassDataBuffer.Release();
            _argumentsBuffer.Release();

            _grassDataBuffer = null;
            _argumentsBuffer = null;
        }

    }
}
