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
                _steeringTorque = Vector3.up * Mathf.Clamp(torqueMagnitude, -_unitData.MaxAngularAcceleration, _unitData.MaxAngularAcceleration);
                _steeringForce = Vector3.ClampMagnitude(_steeringForce, _unitData.MaxAcceleration);
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

        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        private Vector3 _acceleration, _angularAcceleration;

        [SerializeField] private int _recentNormalsCount = 10;

        private Unit _unit;
        private UnitData _unitData;

        private Rigidbody _rigidbody;

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
            Velocity = transform.forward * 10f; // Vector3.zero;
            AngularVelocity = Vector3.zero;
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
        }

        private void FixedUpdate()
        {
            if (_steeringDirection == Vector3.zero)
            {
                return;
            }
            // cast a ray down to get the normal of the ground
            float hitGroundHeight;
            Vector3 hitNormal;
            if (Physics.Raycast(
                _groundCheck.position, Vector3.down,
                out RaycastHit hitDownwards, 10f, _groundLayer
            ))
            {
                Debug.Log("Hit downwards");
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
                    Debug.Log("Hit upwards");
                    hitGroundHeight = hitUpwards.point.y;
                    hitNormal = -hitUpwards.normal;
                }
                else
                {
                    //TODO: unit is somewhere 10f units above or below ground
                    Debug.Log("Hit nowhere");
                    return;
                }
            }

            PlaceSelfOnGround(hitGroundHeight);

            AddNewNormal(hitNormal);
            _normal = GetNormalMovingAverage();
            _tangent = Vector3.Cross(-_normal, transform.right);
            _rigidbody.rotation = Quaternion.LookRotation(_tangent, _normal);

            if (!IsAlignedTowards(_steeringDirection, _activeAlignmentTolerance))
            {
                _acceleration = -Velocity;
            }
            else
            {
                _acceleration = Vector3.ClampMagnitude(
                    _steeringDirection.magnitude * transform.forward,
                    _unitData.MaxAcceleration
                    );
            }

            _angularAcceleration = Vector3.ClampMagnitude(
                _steeringTorque, _unitData.MaxAngularAcceleration
                );

            Vector3 angularFriction = _unitData.FrictionCoefficient * AngularVelocity;
            AngularVelocity += (_angularAcceleration - angularFriction) * Time.fixedDeltaTime;
            AngularVelocity =
                Vector3.ClampMagnitude(AngularVelocity, _unitData.MaxTurnRate);
            _rigidbody.rotation *=
               Quaternion.Euler(AngularVelocity * Time.fixedDeltaTime);

            Vector3 friction = _unitData.FrictionCoefficient * Velocity;
            Velocity += (_acceleration - friction) * Time.fixedDeltaTime;
            Velocity = Vector3.ClampMagnitude(Velocity, _unitData.MaxSpeed);
            _rigidbody.position += Velocity * Time.fixedDeltaTime;

            Debug.Log($"Speed: {Velocity.magnitude}; Angular speed: {AngularVelocity.magnitude}");
            // Debug.Log($"RB position: {_rigidbody.position}; RB rotation: {_rigidbody.rotation}");
        }

        private void PlaceSelfOnGround(float groundHeight)
        {
            Debug.Log($"Ground height: {groundHeight}; RB height: {_rigidbody.position.y}");
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
