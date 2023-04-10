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
            get { return _steeringDirection; }
            set
            {
                _steeringDirection = _steeringForce = value;
                _steeringForce.y = 0f;
                float torqueMagnitude = Vector3.SignedAngle(transform.forward, _steeringForce, Vector3.up);
                torqueMagnitude *= 2f / 3f;
                _steeringTorque = Vector3.up * Mathf.Clamp(torqueMagnitude, -_unitData.MaxTorque, _unitData.MaxTorque);
                _steeringForce = Vector3.ClampMagnitude(_steeringForce, _unitData.MaxForce);
                Debug.Log($"Direction: {_steeringDirection}; Force: {_steeringForce}; Torque: {_steeringTorque}");
            }
        }
        private Vector3 _steeringDirection;
        private Vector3 _steeringForce;
        private Vector3 _steeringTorque;

        private Unit _unit;
        private UnitData _unitData;

        private Rigidbody _rigidbody;

        private Quaternion fromRotation, toRotation;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rigidbody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            enabled = false;
        }

        private void FixedUpdate()
        {
            if (_steeringDirection == Vector3.zero)
                return;

            // Debug.Log($"Steering direction: {_steeringTorque}; Rigidbody velocit: {_rigidbody.velocity}");
            if (
                Vector3.Distance(
                    _steeringDirection.normalized, _rigidbody.velocity.normalized
                    ) > 0.1f
                )
            {
                _rigidbody.AddRelativeTorque(_steeringTorque, ForceMode.Acceleration);
                _rigidbody.angularVelocity = Vector3.ClampMagnitude(
                    _rigidbody.angularVelocity,
                    _unitData.MaxTurnRateDegPerSec
                    );
            }
            else
            {
                _rigidbody.AddRelativeForce(_steeringForce, ForceMode.Acceleration);
                _rigidbody.velocity = Vector3.ClampMagnitude(
                    _rigidbody.velocity,
                    _unitData.MaxSpeed
                    );
            }

        }

        public void StopMovement()
        {
            SteeringDirection = Vector3.zero;
        }

        private void OnDrawGizmos()
        {
            Vector3 from = transform.position + Vector3.up * 2f;
            Color ogColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + _steeringForce);
            Gizmos.color = Color.blue;
            Vector3 perpForce = Vector3.Cross(_steeringForce, Vector3.up);
            Gizmos.DrawLine(
                from + _steeringForce,
                from + _steeringForce +
                perpForce.normalized * _steeringTorque.magnitude
                );
            Gizmos.color = ogColor;
        }
    }
}
