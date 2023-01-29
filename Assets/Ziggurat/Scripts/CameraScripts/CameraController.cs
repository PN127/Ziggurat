using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

namespace Ziggurat
{
    public class CameraController : MonoBehaviour
    {
        private CameraControls _controls;
        private Camera _camera;
        private LayerMask _mask;

        public bool _activeRotate;

        [SerializeField, Range(0.1f, 100f)]
        private float _moveSpeed = 10f;
        [SerializeField, Range(0.1f, 100f)]
        private float _rotateSpeed = 10f;
        [SerializeField, Range(0.1f, 100f)]
        private float _upDownSpeed = 10f;

        private void Awake()
        {
            _controls = new CameraControls();

            _controls.Camera.Scale.performed += OnFocus;
            _controls.Camera.ActiveRotation.performed += OnRightClick;
            _controls.Camera.ActiveRotation.canceled += OnRightClick;
            //    _controls.Camera.Rotate.performed += OnLeftClick;
        }

        private void Start()
        {
            _camera = GetComponent<Camera>();
            _controls.Camera.Enable();
        }

        private void Update()
        {
            OnMoveAndRotation();
        }

        private void OnMoveAndRotation()
        {
            var direction = _controls.Camera.Move.ReadValue<Vector2>();
            transform.position += (transform.forward * direction.y + transform.right * direction.x) * _moveSpeed * Time.deltaTime;

            if (!_activeRotate) return;
            direction = _controls.Camera.Rotate.ReadValue<Vector2>();
            var angle = transform.eulerAngles;
            angle.x -= direction.y * _rotateSpeed * Time.deltaTime;
            angle.y += direction.x * _rotateSpeed * Time.deltaTime;
            angle.z = 0f;

            transform.eulerAngles = angle;
        }
        private void OnFocus(InputAction.CallbackContext context)
        {
            var direction = context.ReadValue<Vector2>().y;
            transform.position += Vector3.up * direction / 12 * _upDownSpeed * Time.deltaTime; 
        }
        private void OnRightClick(InputAction.CallbackContext context)
        {
            _activeRotate = context.performed;
            if (_activeRotate)
                Cursor.lockState = CursorLockMode.Locked;
            else
                Cursor.lockState = CursorLockMode.None;
        }

        private void OnDisable()
        {
            _controls.Camera.Scale.performed -= OnFocus;
            _controls.Camera.ActiveRotation.performed -= OnRightClick;
            _controls.Camera.ActiveRotation.canceled -= OnRightClick;

        }
    }
}