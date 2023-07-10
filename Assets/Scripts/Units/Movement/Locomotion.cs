using UnityEngine;
using StateOfClone.Core;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit))]
    public abstract class Locomotion : MonoBehaviour, IUnitAction
    {
        public SteeringParams SteeringParams { get; set; }

        protected Rigidbody _rb;
        protected LayerMask _groundLayer;
        protected Unit _unit;
        protected UnitData _unitData;

        [SerializeField] protected float _groundDetectionRange = 10f;
        [SerializeField] protected int _recentNormalsCount = 20;
        protected Vector3[] _recentNormals;

        public float CurrentSpeedUnitPerSec { get; protected set; }
        public float CurrentAngularSpeedDegPerSec { get; protected set; }

        protected virtual void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Vector3[_recentNormalsCount];
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
        }

        protected virtual void Start()
        {
            enabled = false;
            if (Physics.Raycast(transform.position - Vector3.up, -Vector3.up,
                out RaycastHit hit, _groundDetectionRange, _groundLayer))
            {
                for (int i = 0; i < _recentNormals.Length; i++)
                {
                    _recentNormals[i] = hit.normal;
                }
                _rb.rotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                    ) * _rb.rotation;
            }
        }

        protected virtual void FixedUpdate()
        {
            UpdateSpeeds();

            Vector3 newPosition =
                _rb.position +
                CurrentSpeedUnitPerSec * Time.fixedDeltaTime * transform.forward;

            Quaternion newRotation = Quaternion.Euler(
                0f, CurrentAngularSpeedDegPerSec * Time.fixedDeltaTime, 0f
                ) * _rb.rotation;

            // Make sure the vehicle sits flush on the ground
            if (Physics.Raycast(
                newPosition + (Vector3.up * _groundDetectionRange), Vector3.down,
                out RaycastHit hit, 2 * _groundDetectionRange, _groundLayer
                ))
            {
                UpdateElevationAndNormal(ref newPosition, ref newRotation, hit);
            }

            ApplySteering(newPosition, newRotation);
        }

        protected abstract void ApplySteering(
            Vector3 newPosition, Quaternion newRotation
            );

        protected virtual void OnDisable()
        {
            StopMovement();
        }

        public virtual void StopMovement()
        {
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
            SteeringParams = SteeringParams.Zero;
        }

        protected Vector3 GetNormalMovingAverage()
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
        protected void AddNewNormal(Vector3 newNormal)
        {
            for (int i = 0; i < _recentNormals.Length - 1; i++)
            {
                _recentNormals[i] = _recentNormals[i + 1];
            }
            _recentNormals[^1] = newNormal;
        }

        protected virtual void UpdateElevationAndNormal(
            ref Vector3 newPosition, ref Quaternion newRotation, RaycastHit hit
            )
        {
            if (_unitData.IsAirborne)
            {
                newPosition.y = hit.point.y + 15f;
            }
            else
            {
                newPosition.y = hit.point.y;
                AddNewNormal(hit.normal);
                newRotation = Quaternion.FromToRotation(
                    transform.up, GetNormalMovingAverage()
                    ) * newRotation;
            }
        }

        protected virtual void UpdateSpeeds()
        {
            // Calculate the new position and rotation
            CurrentSpeedUnitPerSec = Mathf.Clamp(
                SteeringParams.Speed, -_unitData.MaxSpeed, _unitData.MaxSpeed
                );
            CurrentAngularSpeedDegPerSec = Mathf.Clamp(
                SteeringParams.Yaw * _unitData.MaxTurnRate,
                 -_unitData.MaxTurnRate,
                 _unitData.MaxTurnRate
                );
        }

        protected virtual void OnDrawGizmos()
        {
            Vector3 from = transform.position + Vector3.up * 3f;
            Color ogColor = Gizmos.color;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + transform.forward * CurrentSpeedUnitPerSec);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + transform.up * CurrentAngularSpeedDegPerSec);

            Gizmos.color = ogColor;
        }
    }
}