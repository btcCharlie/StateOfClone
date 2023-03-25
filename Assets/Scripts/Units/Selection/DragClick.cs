using UnityEngine;
using UnityEngine.InputSystem;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class DragClick : MonoBehaviour
    {
        [SerializeField] private RectTransform _boxVisual;

        private PlayerInput _playerInput;
        private InputAction _unitDragSelectAction, _unitDragShiftSelectAction;

        private Camera _camera;

        private Rect _selectionBox;

        private Vector2 _startPosition, _endPosition;

        private bool _isDragging = false;

        private void Awake()
        {
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitDragSelectAction = _playerInput.actions["SelectUnit"];
            _unitDragShiftSelectAction = _playerInput.actions["AddToSelection"];
        }

        void Start()
        {
            _camera = Camera.main;

            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;
            DrawVisual();
        }

        private void Update()
        {
            if (_isDragging)
            {
                _endPosition = Mouse.current.position.ReadValue();
                DrawVisual();
                DrawSelection();
            }
        }

        private void OnEnable()
        {
            _unitDragSelectAction.Enable();
            _unitDragShiftSelectAction.Enable();

            _unitDragSelectAction.started += OnUnitDragSelectStarted;
            _unitDragSelectAction.performed += OnUnitDragSelectPerformed;
            _unitDragSelectAction.canceled += OnUnitDragSelectCanceled;
            _unitDragShiftSelectAction.started += OnUnitDragShiftSelect;
        }

        private void OnDisable()
        {
            _unitDragSelectAction.Disable();
            _unitDragShiftSelectAction.Disable();

            _unitDragSelectAction.started -= OnUnitDragSelectStarted;
            _unitDragSelectAction.performed -= OnUnitDragSelectPerformed;
            _unitDragSelectAction.canceled -= OnUnitDragSelectCanceled;
            _unitDragShiftSelectAction.started -= OnUnitDragShiftSelect;
        }

        private void OnUnitDragSelectStarted(InputAction.CallbackContext context)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                return;

            _startPosition = Mouse.current.position.ReadValue();
            _isDragging = true;
        }

        private void OnUnitDragSelectPerformed(InputAction.CallbackContext context)
        {
            _endPosition = Mouse.current.position.ReadValue();
        }

        private void OnUnitDragSelectCanceled(InputAction.CallbackContext context)
        {
            if (Keyboard.current.leftShiftKey.isPressed)
                return;

            SelectUnits();

            _isDragging = false;
            _startPosition = Vector2.zero;
            _endPosition = Vector2.zero;
            DrawVisual();
        }

        private void OnUnitDragShiftSelect(InputAction.CallbackContext context)
        {
            _endPosition = Mouse.current.position.ReadValue();
        }

        private void DrawVisual()
        {
            Vector2 boxStart = _startPosition;
            Vector2 boxEnd = _endPosition;

            Vector2 boxCenter = (boxStart + boxEnd) / 2f;
            _boxVisual.position = boxCenter;

            Vector2 boxSize = new(
                Mathf.Abs(boxStart.x - boxEnd.x), Mathf.Abs(boxStart.y - boxEnd.y)
                );
            _boxVisual.sizeDelta = boxSize;
        }

        private void DrawSelection()
        {
            if (Mouse.current.position.ReadValue().x < _startPosition.x)
            {
                // dragging left
                _selectionBox.xMin = Mouse.current.position.ReadValue().x;
                _selectionBox.xMax = _startPosition.x;
            }
            else
            {
                // dragging right
                _selectionBox.xMin = _startPosition.x;
                _selectionBox.xMax = Mouse.current.position.ReadValue().x;
            }

            if (Mouse.current.position.ReadValue().y < _startPosition.y)
            {
                // dragging down
                _selectionBox.yMin = Mouse.current.position.ReadValue().y;
                _selectionBox.yMax = _startPosition.y;
            }
            else
            {
                // dragging up
                _selectionBox.yMin = _startPosition.y;
                _selectionBox.yMax = Mouse.current.position.ReadValue().y;
            }
        }

        private void SelectUnits()
        {
            foreach (ISelectable unit in SelectionManager.Instance.Units)
            {
                Vector2 screenPosition = _camera.WorldToScreenPoint(unit.gameObject.transform.position);
                if (_selectionBox.Contains(screenPosition))
                {
                    SelectionManager.Instance.DragSelect(unit);
                }
            }
        }
    }
}
