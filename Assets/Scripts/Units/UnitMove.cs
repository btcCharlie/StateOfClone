using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
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

        private Coroutine _moveCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _unit = GetComponent<Unit>();

            _playerInput = CustomInputManager.Instance.PlayerInput;
            _unitMoveAction = _playerInput.actions["MoveUnit"];
        }

        private void OnEnable()
        {
            _unitMoveAction.Enable();
            _unitMoveAction.performed += OnUnitMove;
            _unit.OnSelected.AddListener(OnSelected);
            _unit.OnDeselected.AddListener(OnDeselected);
        }

        private void OnDisable()
        {
            _unitMoveAction.Disable();
            _unitMoveAction.performed -= OnUnitMove;
            _unit.OnSelected.RemoveListener(OnSelected);
            _unit.OnDeselected.RemoveListener(OnDeselected);
        }

        private void OnSelected()
        {
            enabled = true;
        }

        private void OnDeselected()
        {
            enabled = false;
        }

        private void OnUnitMove(InputAction.CallbackContext context)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _groundLayer))
            {
                StopAllCoroutines();
                _moveCoroutine = StartCoroutine(MoveTo_Coroutine(hit.point));
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
