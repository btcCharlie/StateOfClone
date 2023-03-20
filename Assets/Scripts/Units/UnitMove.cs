using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using StateOfClone.Core;
using System;

namespace StateOfClone.Units
{
    public class UnitMove : MonoBehaviour, IUnitAction
    {
        [SerializeField] private LayerMask _groundLayer;

        private PlayerInput _playerInput;
        private InputAction _unitMoveAction;

        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;

            _playerInput = MyInputManager.Instance.PlayerInput;
            _unitMoveAction = _playerInput.actions["MoveUnit"];
        }

        private void OnEnable()
        {
            _unitMoveAction.Enable();
            _unitMoveAction.performed += OnUnitMove;
            GetComponent<Unit>().OnSelected.AddListener(OnSelected);
        }

        private void OnDisable()
        {
            _unitMoveAction.Disable();
            _unitMoveAction.performed -= OnUnitMove;
            GetComponent<Unit>().OnSelected.RemoveListener(OnSelected);
        }

        private void OnSelected()
        {
            enabled = true;
        }

        private void OnUnitMove(InputAction.CallbackContext context)
        {
            Debug.Log("UnitMove.OnUnitMove");
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                Debug.Log("UnitMove.OnUnitMove: hit");
                MoveTo(hit.point);
            }
        }

        private void MoveTo(Vector3 point)
        {
            point.y = transform.position.y;
            transform.position = point;
        }
    }
}
