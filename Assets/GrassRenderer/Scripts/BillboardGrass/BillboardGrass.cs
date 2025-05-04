using UniRx;
using UnityEngine;
namespace GrassRenderer.BillboardGrass
{
    public class BillboardGrass : MonoBehaviour
    {

        private const int GrassDataStride = 28;

        private const int IndirectArgsCount = 5;
        private const int IndirectArgsStride = sizeof(uint) * IndirectArgsCount;

        [Header("Grass Settings")]
        [SerializeField] private int _resolution = 128;
        [SerializeField] private int _scale = 1;
        [SerializeField] private float _displacementStrength = 200f;
        [SerializeField] private Vector3 _quadSize = new (-500f, 200f, 500f);
        [SerializeField] private Texture _heightMap;

        [Header("Shape")]
        [SerializeField] private Mesh _grassMesh;

        [Header("Shader")]
        [SerializeField] private Material _grassMaterial;
        [SerializeField] private string _positionBufferPropertyName = "_PositionBuffer";
        [SerializeField] private string _rotationPropertyName = "_Rotation";

        [Header("Compute Shader")]
        [SerializeField] private string _computeShaderPath = "ComputeShaders/GrassPositionsCalculator";
        [SerializeField] private string _dimensionsPropertyName = "_Dimensions";
        [SerializeField] private string _scalePropertyName = "_Scale";
        [SerializeField] private string _displacementStrengthPropertyName = "_DisplacementStrength";
        [SerializeField] private string _heightMapPropertyName = "_HeightMap";
        [SerializeField] private string _grassDataBufferPropertyName = "_GrassDataBuffer";
        [SerializeField] private string _grassMaterialPropertyName = "_GrassMaterial";
        [SerializeField] private string _grassMeshPropertyName = "_GrassMesh";

        private ComputeShader _grassInitializationShader;
        private ComputeBuffer _grassDataBuffer;
        private ComputeBuffer _argumentsBuffer;

        private Material _grassMaterial1;
        private Material _grassMaterial2;
        private Material _grassMaterial3;

        private void Awake()
        {
            _resolution *= _scale;

            _grassInitializationShader = Resources.Load<ComputeShader>(_computeShaderPath);
            _grassDataBuffer = new ComputeBuffer(_resolution * _resolution, GrassDataStride);
            _argumentsBuffer = new ComputeBuffer(1, IndirectArgsStride, ComputeBufferType.IndirectArguments);

            UpdateGrassBuffer();
            Observable.EveryUpdate().Subscribe(_ => OnGrassUpdate()).AddTo(this);
        }

        private void UpdateGrassBuffer()
        {
            UpgradeGrassInitialization();
            UpgradeGrassArguments();
            UpgradeGrassMaterials();
        }

        private void UpgradeGrassInitialization()
        {
            _grassInitializationShader.SetInt(_dimensionsPropertyName, _resolution);
            _grassInitializationShader.SetInt(_scalePropertyName, _scale);
            _grassInitializationShader.SetBuffer(0, _grassDataBufferPropertyName, _grassDataBuffer);
            _grassInitializationShader.SetTexture(0, _heightMapPropertyName, _heightMap);
            _grassInitializationShader.SetFloat(_displacementStrengthPropertyName, _displacementStrength);
            _grassInitializationShader.Dispatch(0,
                Mathf.CeilToInt(_resolution / 8f), Mathf.CeilToInt(_resolution / 8f), 1);
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
            _grassMaterial1.SetFloat(_displacementStrengthPropertyName, _displacementStrength);

            _grassMaterial2 = new Material(_grassMaterial);
            _grassMaterial2.SetBuffer(_positionBufferPropertyName, _grassDataBuffer);
            _grassMaterial2.SetFloat(_rotationPropertyName, 50.0f);
            _grassMaterial2.SetFloat(_displacementStrengthPropertyName, _displacementStrength);

            _grassMaterial3 = new Material(_grassMaterial);
            _grassMaterial3.SetBuffer(_positionBufferPropertyName, _grassDataBuffer);
            _grassMaterial3.SetFloat(_rotationPropertyName, -50.0f);
            _grassMaterial3.SetFloat(_displacementStrengthPropertyName, _displacementStrength);
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
