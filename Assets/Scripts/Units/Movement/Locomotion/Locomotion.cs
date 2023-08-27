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

        private float _midTurnRate;
        private float _speedStretch;

        [SerializeField] protected float _airborneAltitude = 15f;
        [SerializeField] protected float _groundDetectionRange = 10f;
        [SerializeField] protected int _recentNormalsCount = 10;
        private Queue<Vector3> _recentNormals;

        [SerializeField] protected int _recentSpeedsCount = 5;
        private Queue<float> _recentSpeeds;

        public float CurrentSpeedUnitPerSec { get; protected set; }
        public float CurrentAngularSpeedDegPerSec { get; protected set; }

        protected virtual void Awake()
        {
            _unit = GetComponent<Unit>();
            _ud = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Queue<Vector3>();
            _recentSpeeds = new Queue<float>();
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
        }

        protected virtual void Start()
        {
            enabled = false;
            if (IsOnGround(transform.position, out RaycastHit hit))
            {
                for (int i = 0; i < _recentNormalsCount; i++)
                {
                    _recentNormals.Enqueue(hit.normal);
                }
                _rb.rotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                    ) * _rb.rotation;
            }

            for (int i = 0; i < _recentSpeedsCount; i++)
            {
                _recentSpeeds.Enqueue(0f);
            }

            _actualMaxSpeed = _ud.MaxSpeed;
            _midTurnRate = (_ud.MaxTurnRate - _ud.MinTurnRate) / 2f + _ud.MinTurnRate;
            float expKMinMid = Mathf.Exp(
                _ud.SpeedCurveSlant * (_ud.MinTurnRate - _midTurnRate)
                );
            _speedStretch =
                2f * (_ud.MinSpeed - _ud.MaxSpeed) * expKMinMid / (expKMinMid - 1f);
        }

        protected virtual void FixedUpdate()
        {
            UpdateSpeeds();

            Vector3 newPosition =
                _rb.position +
                GetSpeedMovingAverage() * Time.fixedDeltaTime * transform.forward;

            Quaternion newRotation = Quaternion.Euler(
                0f, CurrentAngularSpeedDegPerSec * Time.fixedDeltaTime, 0f
                ) * _rb.rotation;

            // Make sure the vehicle sits flush on the ground
            if (IsOnGround(newPosition, out RaycastHit hit))
            {
                UpdateElevationAndNormal(ref newPosition, ref newRotation, hit);
            }

            ApplySteering(newPosition, newRotation);

            AddNewSpeed(CurrentSpeedUnitPerSec);
            _actualMaxSpeed = GetMaxSpeedAtTurnRate(CurrentAngularSpeedDegPerSec);
        }

        private bool IsOnGround(Vector3 position, out RaycastHit hit)
        {
            return Physics.Raycast(
                position + (Vector3.up * _groundDetectionRange), Vector3.down,
                out hit, 2 * _groundDetectionRange, _groundLayer);
        }

        public virtual float GetMaxSpeedAtTurnRate(float turnRate)
        {
            turnRate = Mathf.Clamp(
                Mathf.Abs(turnRate), _ud.MinTurnRate, _ud.MaxTurnRate
                );

            return
                (_ud.MaxSpeed - _ud.MinSpeed + _speedStretch) /
                (1f + Mathf.Exp(-_ud.SpeedCurveSlant * (-turnRate + _midTurnRate))) +
                _ud.MinSpeed - _speedStretch / 2f;
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

        protected void OnDisable()
        {
            ClearMovementInput();
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
            while (GetSpeedMovingAverage() != 0f)
            {
                ClearMovementInput();
                yield return waitForFixedUpdate;
            }
            enabled = false;
        }

        protected Vector3 GetNormalMovingAverage()
        {
            return
                _recentNormals.Count > 0 ?
                (_recentNormals.Sum() / _recentNormals.Count) : Vector3.zero;
        }

        protected float GetSpeedMovingAverage()
        {
            return
                _recentSpeeds.Count > 0 ?
                (_recentSpeeds.Sum() / _recentSpeeds.Count) : 0f;
        }

        /// <summary>
        /// Moves the moving average of normals one forward and replaces the 
        /// last element with newNormal.
        /// </summary>
        /// <param name="newNormal">The new normal to add</param>
        protected void AddNewNormal(Vector3 newNormal)
        {
            if (_recentNormals.Count != 0)
            {
                _recentNormals.Dequeue();
            }
            _recentNormals.Enqueue(newNormal);
        }

        /// <summary>
        /// Moves the moving average of speeds one forward and replaces the 
        /// last element with newSpeed.
        /// Never add speed calculated by the moving average! It would fall 
        /// into a cycle.
        /// </summary>
        /// <param name="newSpeed">The new speed to add</param>
        protected void AddNewSpeed(float newSpeed)
        {
            if (_recentSpeeds.Count != 0)
            {
                _recentSpeeds.Dequeue();
            }
            _recentSpeeds.Enqueue(newSpeed);
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
                AddNewNormal(hit.normal);
                newRotation = Quaternion.FromToRotation(
                    transform.up, GetNormalMovingAverage()
                    ) * newRotation;
            }
        }

        protected virtual void UpdateSpeeds()
        {
            CurrentSpeedUnitPerSec = GetSpeed(SteeringParams.Speed);

            CurrentAngularSpeedDegPerSec =
                GetTurnRateFromDeviation(SteeringParams.Yaw);
        }

        protected float GetSpeed(float speedDeviation)
        {
            return Mathf.Clamp(
                speedDeviation,
                -_actualMaxSpeed,
                _actualMaxSpeed
                );
        }

        /// <summary>
        /// Converts the provided angle deviation in degrees to a turn rate in
        /// degrees per second. Uses a polynomial expression: 
        /// (minTurn - maxTurn) * (1 - yawDeviation / maxTurn) ^ yawCurveScale + maxTurn
        /// Prevents slow turning speeds at low deviations.
        /// </summary>
        /// <param name="yawDeviation">The steering signal for horizontal turning</param>
        /// <returns>The actual turn rate of the vehicle bound by its limits</returns>
        public float GetTurnRateFromDeviation(float yawDeviation)
        {
            float unboundTurnRate = yawDeviation switch
            {
                float when yawDeviation > 0 =>
                    ((_ud.MinTurnRate - _ud.MaxTurnRate) *
                    Mathf.Pow(
                        1 - yawDeviation / _ud.MaxTurnRate,
                        _ud.YawCurveScale
                        )) +
                    _ud.MaxTurnRate,
                float when yawDeviation < 0 =>
                    (_ud.MaxTurnRate - _ud.MinTurnRate) *
                    Mathf.Pow(
                        1 + yawDeviation / _ud.MaxTurnRate,
                        _ud.YawCurveScale
                        ) -
                    _ud.MaxTurnRate,
                _ => 0f,
            };

            return Mathf.Clamp(
                unboundTurnRate,
                 -_ud.MaxTurnRate,
                 _ud.MaxTurnRate
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