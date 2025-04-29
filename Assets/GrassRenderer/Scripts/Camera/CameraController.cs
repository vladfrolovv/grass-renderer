using UniRx;
using UnityEngine;
namespace GrassRenderer.Camera
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private float _sensitivity = 3f;

        [SerializeField] private float _minX = -60f;
        [SerializeField] private float _maxX = 60f;

        [SerializeField] private float _minY = -90f;
        [SerializeField] private float _maxY = 90f;

        private float _xRotation = 0f;
        private float _yRotation = 0f;

        private void Awake()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            Vector3 startEuler = transform.rotation.eulerAngles;
            _xRotation = startEuler.x;
            _yRotation = startEuler.y;

            Observable.EveryUpdate()
                .Subscribe(_ => Look())
                .AddTo(this);
        }

        private void Look()
        {
            float mouseX = Input.GetAxis("Mouse X") * _sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _sensitivity;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, _minX, _maxX);

            _yRotation += mouseX;
            _yRotation = Mathf.Clamp(_yRotation, _minY, _maxY);

            transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0f);
        }
    }
}
