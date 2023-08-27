using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

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

        private List<Vector3> _path;

        private void Awake()
        {
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitMoveAction = _playerInput.actions["MoveUnit"];
            _unit = GetComponent<Unit>();
            _rigidbody = GetComponent<Rigidbody>();
            _locomotion = GetComponent<Locomotion>();
            _actionSelector = GetComponent<ActionSelector>();

            _path = new List<Vector3>();
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
            if (_path == null || _path.Count == 0)
            {
                DisableIfDeselected();
                _locomotion.StopMovement();
                return;
            }
            _locomotion.enabled = true;

            Vector3 target = _path[^1];
            if (Vector3.Distance(_rigidbody.position, target) < 0.5f)
            {
                Debug.Log("Reached target");
                _path.RemoveAt(_path.Count - 1);
                return;
            }

            SteeringParams steeringParams = SteeringParams.Zero;
            foreach (SteeringBehavior steering in _actionSelector.Behaviors)
            {
                steeringParams += steering.GetSteering(_rigidbody.position, target);
            }
            if (_actionSelector.Behaviors.Count == 0)
            {
                Debug.LogWarning($"Unit '{gameObject.name}' does not have any steering attached");
            }
            else
            {
                steeringParams /= (float)_actionSelector.Behaviors.Count;
                _locomotion.SteeringParams = steeringParams;
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
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                _path.Clear();
                //! this should first calculate waypoints to the target, putting in 
                //! the target for simplicity
                _path.Add(hit.point);
            }
        }

        private void OnDrawGizmos()
        {
            if (_path == null || _path.Count == 0)
                return;

            // draw the waypoints from path as small red spheres with the 
            // currently active waypoint (the last one) as a larger red sphere
            // also, draw a line between the waypoints, ending at the transform's
            // current position
            Color prevColor = Gizmos.color;
            Gizmos.color = Color.green;
            Vector3 offset = Vector3.up;
            for (int i = 0; i < _path.Count - 1; i++)
            {
                Gizmos.DrawSphere(_path[i] + offset, 0.5f);
                Gizmos.DrawLine(_path[i] + offset, _path[i + 1] + offset);
            }
            Gizmos.DrawSphere(_path[^1] + offset, 1f);
            Gizmos.DrawLine(transform.position + offset, _path[^1] + offset);
            Gizmos.color = prevColor;
        }
    }
}
