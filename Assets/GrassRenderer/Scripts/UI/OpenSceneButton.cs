using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace GrassRenderer.UI
{
    [RequireComponent(typeof(Button))]
    public class OpenSceneButton : MonoBehaviour
    {

        [SerializeField] private SceneType _sceneType;

        private void Awake()
        {
            Button button = GetComponent<Button>();
            button.OnClickAsObservable().Subscribe(delegate
            {
                SceneManager.LoadScene((int)_sceneType);
            }).AddTo(this);
        }

    }
}
