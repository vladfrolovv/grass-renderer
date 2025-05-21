using GrassRenderer.Utilities;
using UnityEditor;
using UnityEngine;
namespace GrassRenderer.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public class HideIfDrawer : PropertyDrawer
    {

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return ShouldShow(property)
                ? EditorGUI.GetPropertyHeight(property, label, true)
                : 0f;                      // zero height = hidden
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (ShouldShow(property))
                EditorGUI.PropertyField(position, property, label, true);
        }

        // ---------- helpers ----------
        private bool ShouldShow(SerializedProperty property)
        {
            var attr      = (HideIfAttribute)attribute;
            var boolProp  = property.serializedObject.FindProperty(attr.boolFieldName);
            // If we couldn’t find the bool, fail “open” so the field is still visible
            return boolProp == null || boolProp.boolValue;
        }

    }
}
