using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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

        private bool _isSelected = false;

        private bool _isMoving = false;
        private List<Vector3> _path;

        private void Awake()
        {
            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitMoveAction = _playerInput.actions["MoveUnit"];
            _unit = GetComponent<Unit>();
            _rigidbody = GetComponent<Rigidbody>();

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
            if (!_isMoving)
            {
                enabled = _isSelected;
                return;
            }

            if (_path.Count == 0)
            {
                Debug.Log("Path is empty");
                _isMoving = false;
                return;
            }

            Vector3 currentWaypoint = _path[^1];
            if (Vector3.Distance(transform.position, currentWaypoint) < 0.1f)
            {
                _path.RemoveAt(_path.Count - 1);
                return;
            }

            Vector3 direction = (currentWaypoint - transform.position).normalized;
            _rigidbody.MovePosition(
                transform.position + 10f * Time.fixedDeltaTime * direction
                );
        }

        private void OnEnable()
        {
            _unitMoveAction.performed += OnUnitMove;
        }

        private void OnDisable()
        {
            _unitMoveAction.performed -= OnUnitMove;
        }

        private void OnDestroy()
        {
            _unit.OnSelected.RemoveListener(OnSelected);
            _unit.OnDeselected.RemoveListener(OnDeselected);
        }

        private void OnSelected()
        {
            enabled = _isSelected = true;
        }

        private void OnDeselected()
        {
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
                _isMoving = true;
            }
        }

        private IEnumerator MoveTo_Coroutine(Vector3 point)
        {
            float travelSpeed = 10f;
            point.y = transform.position.y;
            yield return LookAt_Coroutine(point);

            yield return new WaitForSeconds(0.1f);

            while (Vector3.Distance(transform.position, point) > 0.1f)
            {
                Vector3 direction = (point - transform.position).normalized;
                // transform.localRotation = Quaternion.LookRotation(direction);
                transform.localPosition += Time.deltaTime * travelSpeed * direction;
                yield return null;
            }
        }

        private IEnumerator LookAt_Coroutine(Vector3 point)
        {
            float turnSpeedDegPerSec = 50f;

            Quaternion fromRotation = transform.rotation;
            Quaternion toRotation =
                Quaternion.LookRotation(point - transform.position, transform.up);
            float angleToRotate = Quaternion.Angle(fromRotation, toRotation);

            if (angleToRotate > 0f)
            {
                float speed = turnSpeedDegPerSec / angleToRotate;

                for (
                    float t = Time.deltaTime * speed;
                    t < 1f;
                    t += Time.deltaTime * speed
                )
                {
                    transform.rotation = Quaternion.Slerp(fromRotation, toRotation, t);
                    yield return null;
                }
            }
        }
    }
}
