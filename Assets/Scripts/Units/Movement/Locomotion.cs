using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit))]
    public class Locomotion : MonoBehaviour, IUnitAction
    {
        public SteeringParams SteeringParams { get; set; }

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

        // public struct SteeringParams
        // {
        //     public Vector2 Turning { get; set; }
        //     public float Thrust { get; set; }

        //     public SteeringParams(Vector2 turning, float thrust)
        //     {
        //         Turning = turning;
        //         Thrust = thrust;
        //     }

        //     public static SteeringParams Zero => new(Vector2.zero, 0f);
        // }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Vector3[_recentNormalsCount];
            Velocity = Vector3.zero;
            AngularVelocity = Vector3.zero;
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

        private void FixedUpdate()
        {
            Vector3 turn = Quaternion.Euler(
                0, _unitData.MaxTurnRate * SteeringParams.Yaw * Time.fixedDeltaTime, 0
                ) * _rb.transform.forward;
            Vector3 desiredVelocity = Mathf.Clamp(
                SteeringParams.Speed, -_unitData.MaxSpeed, _unitData.MaxSpeed
                ) * turn.normalized;

            Velocity = desiredVelocity;

            Vector3 nextPosition = _rb.position + desiredVelocity * Time.fixedDeltaTime;

            // Get normal of the terrain at the next position
            Vector3 normal;
            if (Physics.Raycast(
                nextPosition + Vector3.up, Vector3.down, out RaycastHit hit
                ))
            {
                normal = hit.normal;
            }
            else
            {
                normal = Vector3.up;
            }

            // Project the desired direction onto the horizontal plane
            Vector3 horizontalDesiredDirection = Vector3.ProjectOnPlane(
                desiredVelocity, normal
                );

            // Calculate the rotation to orient towards the desired direction
            Quaternion toTarget = Quaternion.LookRotation(
                horizontalDesiredDirection, normal
                );

            // Update position and rotation
            _rb.MovePosition(nextPosition);
            Quaternion rotation = Quaternion.Slerp(_rb.rotation, toTarget,
                            _unitData.MaxTurnRate * Time.fixedDeltaTime);
            _rb.MoveRotation(rotation);
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