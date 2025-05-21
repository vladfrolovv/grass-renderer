using UnityEngine;
namespace GrassRenderer.Utilities
{
    public class HideIfAttribute : PropertyAttribute
    {

        public readonly string boolFieldName;

        public HideIfAttribute(string boolFieldName)
        {
            this.boolFieldName = boolFieldName;
        }

    }
}
