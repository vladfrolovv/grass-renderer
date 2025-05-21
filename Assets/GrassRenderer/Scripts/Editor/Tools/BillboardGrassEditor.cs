using UnityEditor;
using UnityEngine;
namespace GrassRenderer.Editor.Tools
{
    [CustomEditor(typeof(BillboardGrass.BillboardGrassGenerator))]
    public class BillboardGrassEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BillboardGrass.BillboardGrassGenerator billboardGrassGenerator = (BillboardGrass.BillboardGrassGenerator)target;

            if (GUILayout.Button("Update"))
            {
                billboardGrassGenerator.UpdateGrassBuffer();
            }
        }

    }
}
