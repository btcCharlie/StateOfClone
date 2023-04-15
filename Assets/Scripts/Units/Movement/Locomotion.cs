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
                // Debug.Log($"Direction: {_steeringDirection}; Force: {_steeringForce}; Torque: {_steeringTorque}");
            }
        }
        private Vector3 _steeringDirection;
        private Vector3 _steeringForce;
        private Vector3 _steeringTorque;
        private Vector3 _tangent, _normal;

        [SerializeField] private Transform _groundCheck;

        private float _activeAlignmentTolerance;
        [SerializeField] private float _upperAlignmentTolerance = 40f; // upper threshold angle for alignment check
        [SerializeField] private float _lowerAlignmentTolerance = 1f; // lower threshold angle for alignment check
        [SerializeField] private float _stopSpeed = 0.1f; // distance threshold for stopping movement

        public Vector3 Velocity { get; private set; }
        private Vector3 _angularVelocity;

        [SerializeField] private int _recentNormalsCount = 10;

        private Unit _unit;
        private UnitData _unitData;

        private Rigidbody _rigidbody;

        private Quaternion fromRotation, toRotation;

        private LayerMask _groundLayer;

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
            _angularVelocity = Vector3.zero;
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
            _angularVelocity = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (_steeringDirection == Vector3.zero)
                return;

            // cast a ray down to get the normal of the ground
            if (!Physics.Raycast(
                _groundCheck.position, Vector3.down,
                out RaycastHit hit, 10f, _groundLayer
            ))
            {
                //* probably in the air - manage somehow later
                return;
            }

            UpdateRecentNormals(hit.normal);

            _normal = GetNormalMovingAverage();
            _tangent = Vector3.Cross(-_normal, transform.right);
            _rigidbody.transform.rotation = Quaternion.LookRotation(_tangent, _normal);

            // check if the units heading is misaligned with the steering direction
            if (!IsAlignedTowards(_steeringDirection, _activeAlignmentTolerance))
            {
                // stop the movement and rotate towards the steering direction
                _rigidbody.transform.rotation = Quaternion.RotateTowards(
                    _rigidbody.transform.rotation,
                    Quaternion.LookRotation(_steeringDirection, transform.up),
                    _unitData.MaxTurnRate * Time.fixedDeltaTime
                );
                // Debug.Log("Not aligned towards target - rotated up to " +
                //     $"{_unitData.MaxTurnRate * Time.fixedDeltaTime} degrees");
            }
            else
            {
                // move forward and rotate towards the steering direction at the 
                // same time
                Velocity +=
                    _steeringForce.magnitude * Time.fixedDeltaTime * transform.forward;
                // limit the velocity to the maximum speed
                Velocity = Vector3.ClampMagnitude(Velocity, _unitData.MaxSpeed);

                Quaternion newRotation = Quaternion.RotateTowards(
                    _rigidbody.transform.rotation,
                    Quaternion.LookRotation(Velocity, transform.up),
                    _unitData.MaxTurnRate * Time.fixedDeltaTime
                );
                Vector3 newPosition =
                    _rigidbody.transform.position + Velocity * Time.fixedDeltaTime;

                _rigidbody.transform.SetPositionAndRotation(newPosition, newRotation);

                Debug.Log($"Velocity: {Velocity}");

                // Debug.Log("Aligned towards target - moved forward by " +
                //     $"{_steeringForce.magnitude * Time.fixedDeltaTime} units and " +
                //     $"rotated up to {_unitData.MaxTurnRate * Time.fixedDeltaTime} degrees");
            }

            _rigidbody.transform.position = new Vector3(
                _rigidbody.transform.position.x,
                hit.point.y,
                _rigidbody.transform.position.z
            );
        }

        private bool IsAlignedTowards(Vector3 direction, float toleranceDegrees)
        {
            Vector3 forward = transform.forward;
            direction.y = forward.y = 0f;
            float angleDifference = Vector3.Angle(forward, direction);
            Debug.Log($"Angle distance to align: {angleDifference}");
            Debug.Log($"Active alignment tolerance: {toleranceDegrees}");

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
            SteeringDirection = Vector3.zero;
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

        private void UpdateRecentNormals(Vector3 newNormal)
        {
            for (int i = 0; i < _recentNormals.Length - 1; i++)
            {
                _recentNormals[i] = _recentNormals[i + 1];
            }
            _recentNormals[^1] = newNormal;
        }

        private void OnDrawGizmos()
        {
            Vector3 from = transform.position + Vector3.up * 2f;
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

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(from, from + _tangent * 10f);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(from, from + transform.forward * 10f);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + _normal * 10f);

            Gizmos.color = ogColor;
        }
    }
}
