using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    public class UnitMove : MonoBehaviour, IUnitAction
    {
        [SerializeField] private LayerMask _groundLayer;

        private PlayerInput _playerInput;
        private InputAction _unitMoveAction;

        private Camera _camera;

        private Unit _unit;
        private Rigidbody _rigidbody;
        private Locomotion _locomotion;
        private ActionSelector _actionSelector;

        private bool _isSelected = false;

        private List<TargetInfo> _movementTargets;

        private Vector3 _targetGizmo = Vector3.zero;

        private void Awake()
        {
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitMoveAction = _playerInput.actions["MoveUnit"];
            _unit = GetComponent<Unit>();
            _rigidbody = GetComponent<Rigidbody>();
            _locomotion = GetComponent<Locomotion>();
            _actionSelector = GetComponent<ActionSelector>();

            _movementTargets = new List<TargetInfo>();
        }

        private void Start()
        {
            _camera = Camera.main;

            _unit.OnSelected.AddListener(OnSelected);
            _unit.OnDeselected.AddListener(OnDeselected);

            enabled = false;
        }

        private void FixedUpdate()
        {
            if (_movementTargets == null || _movementTargets.Count == 0)
            {
                DisableIfDeselected();
                _locomotion.StopMovement();
                return;
            }
            _locomotion.enabled = true;

            TargetInfo target = _movementTargets[^1];
            if (Vector3.Distance(_rigidbody.position, target.Position) < 0.5f)
            {
                Debug.Log("Reached target");
                _movementTargets.RemoveAt(_movementTargets.Count - 1);
                return;
            }

            SteeringParams steeringParams = SteeringParams.Zero;
            foreach (SteeringBehavior steering in _actionSelector.Behaviors)
            {
                steeringParams += steering.GetSteering(_rigidbody.position, target);
                _targetGizmo = steeringParams.Target;
            }
            if (_actionSelector.Behaviors.Count == 0)
            {
                Debug.LogWarning($"Unit '{gameObject.name}' does not have any steering attached");
            }
            else
            {
                steeringParams /= (float)_actionSelector.Behaviors.Count;
                _locomotion.Motion.SteeringParams = steeringParams;
            }
        }

        private void DisableIfDeselected()
        {
            enabled = _isSelected;
        }

        private void OnDestroy()
        {
            OnDeselected();
            _unit.OnSelected.RemoveListener(OnSelected);
            _unit.OnDeselected.RemoveListener(OnDeselected);
        }

        private void OnSelected()
        {
            enabled = _isSelected = true;
            _unitMoveAction.performed += OnUnitMove;
        }

        private void OnDeselected()
        {
            _unitMoveAction.performed -= OnUnitMove;
            _isSelected = false;
        }

        private void OnUnitMove(InputAction.CallbackContext context)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                // Detect if the hit object is selectable
                ISelectable selectable = hitInfo.collider.GetComponent<ISelectable>();
                Debug.Log($"Hit collider '{hitInfo.collider.name}'");
                Debug.Log($"Hit transform '{hitInfo.transform.name}'");
                Debug.Log($"Selectable: '{selectable}'");

                if (selectable != null)
                {
                    // Handle selectable object logic here
                    _movementTargets.Clear();
                    _movementTargets.Add(new TargetInfo(hitInfo.transform));
                    return;
                }

                // If not selectable, try to find the ground
                if (Physics.Raycast(ray, out RaycastHit groundHitInfo, hitInfo.distance + 10f, _groundLayer))
                {
                    _movementTargets.Clear();
                    //! this should first calculate waypoints to the target
                    //! putting in the target for simplicity
                    _movementTargets.Add(new TargetInfo(groundHitInfo.point));
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_movementTargets == null || _movementTargets.Count == 0)
                return;

            // draw the waypoints from path as small red spheres with the 
            // currently active waypoint (the last one) as a larger red sphere
            // also, draw a line between the waypoints, ending at the transform's
            // current position
            Color prevColor = Gizmos.color;
            Gizmos.color = Color.green;
            Vector3 offset = Vector3.up;
            for (int i = 0; i < _movementTargets.Count - 1; i++)
            {
                Vector3 start = _movementTargets[i].Type switch
                {
                    TargetType.Ground => _movementTargets[i].Position,
                    TargetType.Selectable => _movementTargets[i].Transform.position,
                    _ => Vector3.zero
                };
                Vector3 stopNext = _movementTargets[i + 1].Type switch
                {
                    TargetType.Ground => _movementTargets[i + 1].Position,
                    TargetType.Selectable => _movementTargets[i + 1].Transform.position,
                    _ => Vector3.zero
                };
                Gizmos.DrawSphere(start + offset, 0.5f);
                Gizmos.DrawLine(start + offset, stopNext + offset);
            }
            Gizmos.DrawSphere(_targetGizmo + offset, 1f);
            Gizmos.DrawLine(transform.position + offset, _targetGizmo + offset);
            Gizmos.color = prevColor;
        }
    }
}
