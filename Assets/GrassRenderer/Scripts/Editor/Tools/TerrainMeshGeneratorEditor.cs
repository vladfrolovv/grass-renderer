using UnityEditor;
using UnityEngine;
namespace GrassRenderer.Editor.Tools
{
    [CustomEditor(typeof(Terrain.TerrainMeshGenerator))]
    public class TerrainMeshGeneratorEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            Terrain.TerrainMeshGenerator terrainMeshGenerator = (Terrain.TerrainMeshGenerator)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate New Terrain"))
            {
                terrainMeshGenerator.GenerateNewTerrain();
            }

            if (GUILayout.Button("Clear Mesh"))
            {
                terrainMeshGenerator.ClearMesh();
            }

            GUILayout.EndHorizontal();
        }

    }
}
