using UnityEngine;
using UnityEngine.InputSystem;

namespace StateOfClone.Units
{
    public class UnitClick : MonoBehaviour
    {
        [SerializeField] private LayerMask _clickableLayer, _groundLayer;

        private Camera _camera;

        private PlayerInput _playerInput;
        private InputAction _unitSelectAction, _unitShiftSelectAction;

        void Awake()
        {
            _camera = Camera.main;
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitSelectAction = _playerInput.actions["SelectUnit"];
            _unitShiftSelectAction = _playerInput.actions["AddToSelection"];
        }

        private void OnEnable()
        {
            _unitSelectAction.Enable();
            _unitShiftSelectAction.Enable();

            _unitSelectAction.performed += OnUnitSelect;
            _unitShiftSelectAction.performed += OnUnitShiftSelect;
        }

        private void OnDisable()
        {
            _unitSelectAction.Disable();
            _unitShiftSelectAction.Disable();

            _unitSelectAction.performed -= OnUnitSelect;
            _unitShiftSelectAction.performed -= OnUnitShiftSelect;
        }

        private void OnUnitSelect(InputAction.CallbackContext context)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                return;

            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _clickableLayer))
            {
                SelectionManager.Instance.ClickSelect(
                    SelectionManager.GetClickableObject(hit)
                    );
            }
            else
            {
                SelectionManager.Instance.DeselectAll();
            }
        }

        private void OnUnitShiftSelect(InputAction.CallbackContext context)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _clickableLayer))
            {
                SelectionManager.Instance.ShiftClickSelect(
                    SelectionManager.GetClickableObject(hit)
                    );
            }
        }
    }
}
