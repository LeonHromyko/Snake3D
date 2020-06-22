using UnityEngine;

namespace GamePlay.Scripts
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        private float _moveSpeed = 10f;

        [SerializeField]
        private float _rotateSpeed = 10f;

        [SerializeField]
        private float _zoomSpeed = 10f;

        [SerializeField]
        private float _fovMin = 5f;

        [SerializeField]
        private float _fovMax = 150f;

        [SerializeField]
        private float _yAngleMin = -45f;

        [SerializeField]
        private float _yAngleMax = 45f;
        
        [SerializeField]
        private Transform _gameFieldRoot;

        private Camera _cam;

        private void Start()
        {
            var settings = FindObjectOfType<GameManager>().Settings;
            var offset= settings.GameFieldSize * settings.GameFieldCellSize;
            
            _cam = GetComponent<Camera>();
            _cam.transform.position = _gameFieldRoot.position + Vector3.one * offset;
            _cam.transform.LookAt(_gameFieldRoot);
        }

        private void Update()
        {
            var horAxis = Input.GetAxis("Horizontal");
            var verAxis = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(horAxis, 0f, verAxis) * _moveSpeed * Time.deltaTime);

            var zoomAxis = Input.GetAxis("Mouse ScrollWheel");
            _cam.fieldOfView = Mathf.Clamp(_cam.fieldOfView - zoomAxis * _zoomSpeed * Time.deltaTime, _fovMin, _fovMax);

            var mouseY = Input.GetAxis("Mouse Y");
            var eulerAngles = transform.eulerAngles;
            eulerAngles.x = Mathf.Clamp(eulerAngles.x - mouseY * _rotateSpeed * Time.deltaTime, _yAngleMin, _yAngleMax);
            transform.eulerAngles = eulerAngles;

            var mouseX = Input.GetAxis("Mouse X");
            transform.Rotate(0f, mouseX * _rotateSpeed * Time.deltaTime, 0f, Space.World);
        }
    }
}