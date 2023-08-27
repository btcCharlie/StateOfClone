using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace StateOfClone.Units
{
    /// <summary>
    /// Physical motion of units.
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public abstract class Locomotion : MonoBehaviour, IUnitAction
    {
        public SteeringParams SteeringParams { get; set; }
        public Vector3 CurrentVelocity
        {
            get { return transform.forward * CurrentSpeedUnitPerSec; }
        }
        public Vector3 Forward
        {
            get { return transform.forward; }
        }

        protected Rigidbody _rb;
        protected LayerMask _groundLayer;
        protected Unit _unit;
        protected UnitData _ud;
        protected float _actualMaxSpeed;

        [SerializeField] protected float _airborneAltitude = 15f;
        [SerializeField] protected float _groundDetectionRange = 10f;
        [SerializeField] protected int _recentNormalsCount = 10;
        [SerializeField] protected int _recentSpeedsCount = 5;

        public float CurrentSpeedUnitPerSec { get; protected set; }
        public float CurrentAngularSpeedDegPerSec { get; protected set; }

        private SmoothingAverageQueue _smoothingQueues;

        public ISpeedCalculator SpeedCalculator { get; protected set; }

        protected virtual void Awake()
        {
            _unit = GetComponent<Unit>();
            _ud = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
            SpeedCalculator = new DefaultSpeedCalculator(_ud);
            _smoothingQueues = new SmoothingAverageQueue(
                _recentSpeedsCount, _recentNormalsCount
            );
        }

        protected virtual void Start()
        {
            enabled = false;
            if (IsOnGround(transform.position, out RaycastHit hit))
            {
                _rb.rotation = Quaternion.FromToRotation(
                        transform.up, hit.normal
                    ) * _rb.rotation;
            }

            _smoothingQueues.Initiliaze(hit.normal, 0f);

            _actualMaxSpeed = _ud.MaxSpeed;
        }

        protected virtual void FixedUpdate()
        {
            UpdateSpeeds();

            Vector3 newPosition =
                _rb.position +
                _smoothingQueues.AverageSpeed() * Time.fixedDeltaTime * transform.forward;

            Quaternion newRotation = Quaternion.Euler(
                    0f, CurrentAngularSpeedDegPerSec * Time.fixedDeltaTime, 0f
                ) * _rb.rotation;

            // Make sure the vehicle sits flush on the ground
            if (IsOnGround(newPosition, out RaycastHit hit))
            {
                UpdateElevationAndNormal(ref newPosition, ref newRotation, hit);
            }

            ApplySteering(newPosition, newRotation);

            _smoothingQueues.AddNewSpeed(CurrentSpeedUnitPerSec);
            _actualMaxSpeed = SpeedCalculator.GetMaxSpeedAtTurnRate(CurrentAngularSpeedDegPerSec);
        }

        private bool IsOnGround(Vector3 position, out RaycastHit hit)
        {
            return Physics.Raycast(
                position + (Vector3.up * _groundDetectionRange), Vector3.down,
                out hit, 2 * _groundDetectionRange, _groundLayer);
        }

        public virtual float GetMaxSpeedAtTurnRate(float turnRate)
        {
            return SpeedCalculator.GetMaxSpeedAtTurnRate(turnRate);
        }

        protected abstract void ApplySteering(
            Vector3 newPosition, Quaternion newRotation
            );

        /// <summary>
        /// Repeatedly clears the movement input until the vehicle stops,
        /// then disables to component to stop calculating new movement.
        /// </summary>
        public void StopMovement()
        {
            ClearMovementInput();
            StartCoroutine(StopMovementAndDisable_Co());
        }

        protected void ClearMovementInput()
        {
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
            SteeringParams = SteeringParams.Zero;
        }

        protected IEnumerator StopMovementAndDisable_Co()
        {
            WaitForFixedUpdate waitForFixedUpdate = new();
            while (_smoothingQueues.AverageSpeed() != 0f)
            {
                ClearMovementInput();
                yield return waitForFixedUpdate;
            }
            enabled = false;
        }

        protected virtual void UpdateElevationAndNormal(
                ref Vector3 newPosition, ref Quaternion newRotation, RaycastHit hit
            )
        {
            if (_ud.IsAirborne)
            {
                newPosition.y = hit.point.y + _airborneAltitude;
            }
            else
            {
                newPosition.y = hit.point.y;
                _smoothingQueues.AddNewNormal(hit.normal);
                newRotation = Quaternion.FromToRotation(
                        transform.up, _smoothingQueues.AverageNormal()
                    ) * newRotation;
            }
        }

        protected virtual void UpdateSpeeds()
        {
            CurrentSpeedUnitPerSec = GetSpeed(SteeringParams.Speed);

            CurrentAngularSpeedDegPerSec = SpeedCalculator.CalculateYawTurnRate(
                SteeringParams.Yaw
            );
        }

        protected float GetSpeed(float speedDeviation)
        {
            return SpeedCalculator.CalculateSpeed(
                speedDeviation,
                    _actualMaxSpeed,
                -_actualMaxSpeed
                );
        }

        protected void OnEnable()
        {
            SpeedCalculator = new DefaultSpeedCalculator(_ud);

            Vector3 currentNormal = _smoothingQueues.AverageNormal();
            _smoothingQueues = new SmoothingAverageQueue(
                _recentSpeedsCount, _recentNormalsCount
            );
            _smoothingQueues.Initiliaze(currentNormal, 0f);
        }

        protected void OnDisable()
        {
            ClearMovementInput();
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