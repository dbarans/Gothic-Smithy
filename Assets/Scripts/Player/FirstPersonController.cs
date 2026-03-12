using UnityEngine;
using UnityEngine.InputSystem;
using Smithy.Core;

namespace Smithy.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float lookSensitivity = 1f;
        [SerializeField] private InputActionAsset inputActions;

        [SerializeField] private float interactRange = 3f;
        [SerializeField] private LayerMask interactLayerMask = -1;
        [SerializeField] private Transform cameraHolder;

        private CharacterController _controller;
        private Vector2 _moveInput;
        private Vector2 _lookInput;
        private float _pitch;
        private InputAction _moveAction;
        private InputAction _lookAction;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            if (cameraHolder == null)
            {
                var cam = GetComponentInChildren<Camera>();
                if (cam != null) cameraHolder = cam.transform;
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (inputActions != null)
            {
                var map = inputActions.FindActionMap("Player");
                if (map != null)
                {
                    _moveAction = map.FindAction("Move");
                    _lookAction = map.FindAction("Look");
                    map.Enable();
                }
            }
        }

        private void Update()
        {
            if (_moveAction != null) _moveInput = _moveAction.ReadValue<Vector2>();
            if (_lookAction != null) _lookInput = _lookAction.ReadValue<Vector2>();

            Move();
            Look();
            TryInteract();
        }

        private void Move()
        {
            Vector3 dir = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            _controller.Move(dir * (moveSpeed * Time.deltaTime));
        }

        private void Look()
        {
            float yaw = _lookInput.x * lookSensitivity;
            _pitch -= _lookInput.y * lookSensitivity;
            _pitch = Mathf.Clamp(_pitch, -89f, 89f);
            transform.Rotate(0f, yaw, 0f);
            if (cameraHolder != null)
                cameraHolder.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
        }

        private void TryInteract()
        {
            bool interactPressed = Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame;
            if (!interactPressed) return;

            if (cameraHolder == null) return;
            var ray = new Ray(cameraHolder.position, cameraHolder.forward);

            if (Physics.Raycast(ray, out var hit, interactRange, interactLayerMask))
            {
                var interactable = hit.collider.GetComponentInParent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact(new InteractContext { Player = gameObject });
                }
            }
        }

        public void OnMove(InputValue value) => _moveInput = value.Get<Vector2>();
        public void OnLook(InputValue value) => _lookInput = value.Get<Vector2>();
        public void OnMove(InputAction.CallbackContext context) => _moveInput = context.ReadValue<Vector2>();
        public void OnLook(InputAction.CallbackContext context) => _lookInput = context.ReadValue<Vector2>();
    }
}
