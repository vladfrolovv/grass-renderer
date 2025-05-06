using UnityEditor;
using UnityEngine;
namespace GrassRenderer.Editor.Tools
{
    [CustomEditor(typeof(BillboardGrass.BillboardGrass))]
    public class BillboardGrassEditor : UnityEditor.Editor
    {

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            BillboardGrass.BillboardGrass billboardGrass = (BillboardGrass.BillboardGrass)target;

            if (GUILayout.Button("Update"))
            {
                billboardGrass.UpdateGrassBuffer();
            }
        }

    }
}
