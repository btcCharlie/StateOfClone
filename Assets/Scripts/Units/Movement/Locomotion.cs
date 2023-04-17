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
        public Vector3 Steering
        {
            get { return _steering; }
            set
            {
                _steering = _steeringForce = value;
                _steeringForce.y = 0f;
                _torque =
                    Vector3.SignedAngle(transform.forward, _steeringForce, Vector3.up);

                float speedMultiplier = 100f * _steeringForce.magnitude / _unitData.MaxSpeed;
                Speed = Mathf.Clamp(
                    speedMultiplier * _unitData.MaxSpeed,
                    -_unitData.MaxSpeed, _unitData.MaxSpeed
                    );

                _steeringTorque = Vector3.up * Mathf.Clamp(
                    _torque,
                    -_unitData.MaxAngularAcceleration,
                    _unitData.MaxAngularAcceleration
                    );
                _steeringForce =
                    Vector3.ClampMagnitude(_steeringForce, _unitData.MaxAcceleration);
            }
        }
        private Vector3 _steering, _steeringForce, _steeringTorque;
        private Vector3 _tangent, _normal;

        [SerializeField] private Transform _groundCheck;

        private float _activeAlignmentTolerance;
        [SerializeField] private float _upperAlignmentTolerance = 40f; // upper threshold angle for alignment check
        [SerializeField] private float _lowerAlignmentTolerance = 1f; // lower threshold angle for alignment check

        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public float Speed { get; private set; }
        private float _thrust;
        private float _torque;

        private Unit _unit;
        private UnitData _unitData;

        private Rigidbody _rigidbody;

        private LayerMask _groundLayer;

        [SerializeField] private int _recentNormalsCount = 10;
        private Vector3[] _recentNormals;

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rigidbody = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Vector3[_recentNormalsCount];
            _activeAlignmentTolerance = _lowerAlignmentTolerance;
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
            Speed = 0f;
        }

        private void Start()
        {
            enabled = false;
            if (Physics.Raycast(
                _groundCheck.position, Vector3.down,
                out RaycastHit hit, 10f, _groundLayer
                ))
            {
                for (int i = 0; i < _recentNormals.Length; i++)
                {
                    _recentNormals[i] = hit.normal;
                }
            }
        }

        private void OnDisable()
        {
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
            Speed = 0f;
        }

        private void FixedUpdate()
        {
            if (_steering == Vector3.zero)
            {
                return;
            }

            float hitGroundHeight;
            Vector3 hitNormal;
            if (Physics.Raycast(
                _groundCheck.position, Vector3.down,
                out RaycastHit hitDownwards, 10f, _groundLayer
            ))
            {
                // Debug.Log("Hit downwards");
                hitGroundHeight = hitDownwards.point.y;
                hitNormal = hitDownwards.normal;
            }
            else
            {
                if (Physics.Raycast(
                    _groundCheck.position - Vector3.up, Vector3.up,
                    out RaycastHit hitUpwards, 10f, _groundLayer
                ))
                {
                    // Debug.Log("Hit upwards");
                    hitGroundHeight = hitUpwards.point.y;
                    hitNormal = -hitUpwards.normal;
                }
                else
                {
                    //TODO: unit is somewhere 10f units above or below ground
                    // Debug.Log("Hit nowhere");
                    return;
                }
            }

            PlaceSelfOnGround(hitGroundHeight);

            AddNewNormal(hitNormal);
            _normal = GetNormalMovingAverage();
            _tangent = Vector3.Cross(-_normal, transform.right);
            _rigidbody.rotation = Quaternion.LookRotation(_tangent, _normal);

            if (!IsAlignedTowards(_steering, _activeAlignmentTolerance))
            {
            }
            else
            {
            }

            // Speed += _thrust * Time.fixedDeltaTime;
            Speed = Mathf.Clamp(Speed, -_unitData.MaxSpeed, _unitData.MaxSpeed);
            Velocity = transform.forward * Speed;
            Quaternion targetRotation = Quaternion.LookRotation(_steering, _normal);

            float steeringMagnitude = new Vector3(_steering.x, 0f, _steering.z).magnitude;

            // Interpolate between the maximum step size and zero based on the angle
            float step = Mathf.Lerp(
                0f, _steeringTorque.magnitude, steeringMagnitude / 3f
                );
            Debug.Log($"Steering mag: {steeringMagnitude}; Step: {step}; Max step: {_steeringTorque.magnitude}");

            // Rotate towards the target using the interpolated step size
            _rigidbody.rotation = Quaternion.RotateTowards(
                _rigidbody.rotation, targetRotation,
                step * Time.fixedDeltaTime
            );

            _rigidbody.position +=
                Time.fixedDeltaTime * Velocity;
        }

        private void PlaceSelfOnGround(float groundHeight)
        {
            _rigidbody.position =
                new Vector3(_rigidbody.position.x, groundHeight, _rigidbody.position.z);
        }

        private bool IsAlignedTowards(Vector3 direction, float toleranceDegrees)
        {
            Vector3 forward = transform.forward;
            direction.y = forward.y = 0f;
            float angleDifference = Vector3.Angle(forward, direction);
            // Debug.Log($"Angle distance to align: {angleDifference}");
            // Debug.Log($"Active alignment tolerance: {toleranceDegrees}");

            if (angleDifference < _lowerAlignmentTolerance)
            {
                _activeAlignmentTolerance = _upperAlignmentTolerance;
            }
            if (angleDifference > _upperAlignmentTolerance)
            {
                _activeAlignmentTolerance = _lowerAlignmentTolerance;
            }

            return angleDifference <= toleranceDegrees;
        }

        public void StopMovement()
        {
            Steering = Vector3.zero;
        }

        private Vector3 GetNormalMovingAverage()
        {
            Vector3 sum = Vector3.zero;
            for (int i = 0; i < _recentNormals.Length; i++)
            {
                sum += _recentNormals[i];
            }
            return sum / _recentNormals.Length;
        }

        private void AddNewNormal(Vector3 newNormal)
        {
            for (int i = 0; i < _recentNormals.Length - 1; i++)
            {
                _recentNormals[i] = _recentNormals[i + 1];
            }
            _recentNormals[^1] = newNormal;
        }

        private void OnDrawGizmos()
        {
            Vector3 from = transform.position + Vector3.up * 3f;
            Color ogColor = Gizmos.color;
            // Gizmos.color = Color.red;
            // Gizmos.DrawLine(from, from + _steeringForce);
            // Gizmos.color = Color.blue;
            // Vector3 perpForce = Vector3.Cross(_steeringForce, Vector3.up);
            // Gizmos.DrawLine(
            //     from + _steeringForce,
            //     from + _steeringForce +
            //     perpForce.normalized * _steeringTorque.magnitude
            //     );

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + Velocity);
            // Gizmos.color = Color.blue;
            // Gizmos.DrawLine(from, from + transform.forward * 10f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + AngularVelocity);

            Gizmos.color = ogColor;
        }
    }
}
