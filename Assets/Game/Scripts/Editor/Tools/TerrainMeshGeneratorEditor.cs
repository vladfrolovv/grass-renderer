using System;
using Game.Scripts.Runtime.Terrain;
using UnityEditor;
using UnityEngine;
namespace Game.Scripts.Editor.Tools
{
    [CustomEditor(typeof(TerrainMeshGenerator))]
    public class TerrainMeshGeneratorEditor : UnityEditor.Editor
    {

        private TerrainMeshGenerator _terrainMeshGenerator;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Generate"))
            {
                _terrainMeshGenerator.Generate();
            }

            if (GUILayout.Button("Clear"))
            {
                _terrainMeshGenerator.Clear();
            }

            GUILayout.EndHorizontal();
        }

        protected void OnEnable()
        {
            _terrainMeshGenerator = (TerrainMeshGenerator) target;
        }

    }
}
