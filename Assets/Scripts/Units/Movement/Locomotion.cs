using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit))]
    public class Locomotion : MonoBehaviour, IUnitAction
    {
        /// <summary>
        /// Local steering direction in 3D space
        /// </summary>
        public Vector3 SteeringDirection
        {
            get { return steeringForce; }
            set
            {
                steeringForce = value;
                steeringForce.y = 0f;
                steeringForce = Vector3.ClampMagnitude(steeringForce, _unitData.MaxForce);
            }
        }
        private Vector3 steeringForce;

        private Unit _unit;
        private UnitData _unitData;

        private Rigidbody _rigidbody;

        private bool _isSelected = false, _isMoving = false;

        private Quaternion fromRotation, toRotation;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            _unit.OnSelected.AddListener(OnSelected);
            _unit.OnDeselected.AddListener(OnDeselected);

            enabled = false;
        }

        private void FixedUpdate()
        {
            if (SteeringDirection == Vector3.zero)
            {
                enabled = _isSelected;
                return;
            }

            if (_rigidbody.)

                _rigidbody.AddRelativeForce(steeringForce, ForceMode.Acceleration);
            _rigidbody.velocity = Vector3.ClampMagnitude(_rigidbody.velocity, _unitData.MaxSpeed);

            // Vector3 direction = (currentWaypoint - transform.position).normalized;
            // toRotation = Quaternion.LookRotation(direction, transform.up);

            // Vector3 toRotationEulers = toRotation.eulerAngles;
            // toRotationEulers.x = transform.rotation.eulerAngles.x;
            // toRotationEulers.z = transform.rotation.eulerAngles.z;
            // toRotation = Quaternion.Euler(toRotationEulers);

            // float angleToTurnDeg = Quaternion.Angle(transform.rotation, toRotation);

            // // tracked vehicle behavior - rotates in place and then moves
            // if (angleToTurnDeg > 1f)
            // {
            //     float turnProgressPerSec = _unitData.MaxTurnRateDegPerSec / angleToTurnDeg;

            //     _rigidbody.MoveRotation(Quaternion.Slerp(
            //         transform.rotation,
            //         toRotation,
            //         turnProgressPerSec * Time.fixedDeltaTime));
            // }
            // else
            // {
            //     _rigidbody.MovePosition(
            //         transform.position + 10f * Time.fixedDeltaTime * transform.forward
            //         );
            // }
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
        }

        private void OnDeselected()
        {
            _isSelected = false;
        }

        public void StopMovement()
        {
            SteeringDirection = Vector3.zero;
        }
    }
}
