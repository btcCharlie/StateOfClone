using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit))]
    public class Locomotion : MonoBehaviour, IUnitAction
    {
        private Rigidbody _rb;
        private LayerMask _groundLayer;
        private Unit _unit;
        private UnitData _unitData;

        [SerializeField] private int _recentNormalsCount = 10;
        [SerializeField] private float _rotationAlignmentThreshold = 50f;
        private Vector3[] _recentNormals;

        // Speed at which the vehicle moves
        public float SpeedUnitsPerSec { get; private set; } = 10f;
        // Speed at which the vehicle turns
        public float TurnSpeedDegreesPerSec { get; set; } = 0.1f;
        // How far below the vehicle to look for the ground
        public float GroundDetectionRangeUnits { get; set; } = 10f;
        // Layer containing the ground
        public Vector3 SteeringDirection { get; set; }
        public Vector3 Velocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Vector3[_recentNormalsCount];
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
            // SpeedUnitsPerSec = 0f;
        }

        private void Start()
        {
            enabled = false;
            if (Physics.Raycast(transform.position, -transform.up,
                out RaycastHit hit, GroundDetectionRangeUnits, _groundLayer))
            {
                for (int i = 0; i < _recentNormals.Length; i++)
                {
                    _recentNormals[i] = hit.normal;
                }
            }
        }

        void FixedUpdate()
        {
            // Cast a ray downwards to detect the ground
            if (Physics.Raycast(transform.position, -transform.up,
                out RaycastHit hit, GroundDetectionRangeUnits, _groundLayer))
            {
                // Rotate the vehicle to match the ground's normal
                Quaternion toRotation = Quaternion.FromToRotation(
                    transform.up, hit.normal) * _rb.rotation;
                _rb.rotation = Quaternion.Slerp(
                    _rb.rotation, toRotation, Time.deltaTime * TurnSpeedDegreesPerSec);
            }

            // Find the rotation that points the vehicle towards the steering direction
            Vector3 flattenedSteeringDirection = new(
                SteeringDirection.x, 0, SteeringDirection.z);
            Quaternion desiredRotation = Quaternion.LookRotation(
                flattenedSteeringDirection);

            // Calculate the angle between the current and desired direction
            float angleDiff = Quaternion.Angle(_rb.rotation, desiredRotation);

            // If the angle difference is small, don't adjust the rotation
            if (angleDiff > _rotationAlignmentThreshold)
            {
                // Use this to calculate a modified turn speed
                float modifiedTurnSpeed = TurnSpeedDegreesPerSec * (angleDiff / 180f);

                // Rotate the vehicle towards the desired rotation
                _rb.rotation = Quaternion.Slerp(
                    _rb.rotation, desiredRotation, Time.deltaTime * modifiedTurnSpeed);
            }

            // Move the vehicle forward
            _rb.position += SpeedUnitsPerSec * Time.fixedDeltaTime * transform.forward;

            Velocity = transform.forward * SpeedUnitsPerSec;
        }

        private void OnDisable()
        {
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
            // SpeedUnitsPerSec = 0f;
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

        /// <summary>
        /// Moves the moving average of normals one forward and replaces the 
        /// last element with newNormal.
        /// </summary>
        /// <param name="newNormal">The new normal to add</param>
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