using UnityEngine;
using UnityEngine.InputSystem;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class UnitClick : MonoBehaviour
    {
        [SerializeField] private LayerMask _clickableLayer, _groundLayer;

        [SerializeField] private PlayerInput playerInput;

        private Camera _camera;

        private InputAction _unitSelectAction, _unitShiftSelectAction, _unitDragSelectAction;

        void Awake()
        {
            _camera = Camera.main;
            _unitSelectAction = playerInput.actions["SelectUnit"];
            _unitShiftSelectAction = playerInput.actions["AddToSelection"];
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
                SelectionManager.Instance.ClickSelect(GetClickableObject(hit));
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
                SelectionManager.Instance.ShiftClickSelect(GetClickableObject(hit));
            }
        }

        private static GameObject GetClickableObject(RaycastHit hit)
        {
            GameObject clickableObject = hit.collider.gameObject;
            while (!clickableObject.TryGetComponent<IClickable>(out _))
            {
                // there has to be a clickable object because we're in
                // a clickable layer - if not, an error is due anyway
                clickableObject = clickableObject.transform.parent.gameObject;
            }

            return clickableObject;
        }
    }
}
