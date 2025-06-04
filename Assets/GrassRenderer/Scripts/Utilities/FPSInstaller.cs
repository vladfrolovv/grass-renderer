using UnityEngine;
namespace GrassRenderer.Utilities
{
    public class FPSInstaller : MonoBehaviour
    {

        private void Awake()
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
        }

    }
}
