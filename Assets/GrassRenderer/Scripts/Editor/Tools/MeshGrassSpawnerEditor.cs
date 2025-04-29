using GrassRenderer.MeshGrass;
using UnityEditor;
using UnityEngine;
namespace GrassRenderer.Editor.Tools
{
    [CustomEditor(typeof(MeshGrassSpawner))]
    public class MeshGrassSpawnerEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            MeshGrassSpawner spawner = (MeshGrassSpawner)target;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Generate Grass"))
            {
                spawner.GenerateGrass();
            }

            if (GUILayout.Button("Clear Grass"))
            {
                spawner.ClearGrass();
            }
            GUILayout.EndHorizontal();
        }

    }
}
