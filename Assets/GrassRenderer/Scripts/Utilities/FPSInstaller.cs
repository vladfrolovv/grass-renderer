using UnityEngine;
namespace GrassRenderer.Utilities
{
    public class FPSInstaller : MonoBehaviour
    {

        private void Awake()
        {
            Application.targetFrameRate = int.MaxValue;
            QualitySettings.vSyncCount = 0;
        }

    }
}
