using UnityEngine;
using System;
using System.Collections;

namespace StateOfClone.Units
{
    /// <summary>
    /// Physical motion of units.
    /// </summary>
    [RequireComponent(typeof(Unit))]
    public class Locomotion : MonoBehaviour
    {
        private Rigidbody _rb;
        private LayerMask _groundLayer;
        private Unit _unit;
        private UnitData _ud;

        [SerializeField] private float _groundDetectionRange = 10f;
        [SerializeField] private int _recentNormalsCount = 10;
        [SerializeField] private int _recentSpeedsCount = 5;

        private SmoothingAverageQueue _smoothingQueues;

        public ISpeedCalculator SpeedCalculator { get; private set; }

        [SerializeField] private MotionType motionTypeSelection;

        public IMotionType Motion { get; private set; }

        private void Awake()
        {
            _unit = GetComponent<Unit>();
            _ud = _unit ? _unit.UnitData :
                throw new Exception("Unit component missing or UnitData is null");
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _smoothingQueues = new SmoothingAverageQueue(
                _recentSpeedsCount, _recentNormalsCount
            );

            Motion = MotionTypeFactory.CreateMotion(
                motionTypeSelection, _ud
                );
        }

        private void Start()
        {
            enabled = false;
            if (IsOnGround(transform.position, out RaycastHit hit))
            {
                _rb.rotation = Quaternion.FromToRotation(
                        transform.up, hit.normal
                    ) * _rb.rotation;

                _smoothingQueues.Initiliaze(hit.normal, 0f);
            }
        }

        private void FixedUpdate()
        {
            Motion.UpdateSpeeds();

            Vector3 newPosition =
                _rb.position +
                _smoothingQueues.AverageSpeed() * Time.fixedDeltaTime * transform.forward;

            Quaternion newRotation = Quaternion.Euler(
                    0f, Motion.CurrentAngularSpeedDegPerSec * Time.fixedDeltaTime, 0f
                ) * _rb.rotation;

            if (IsOnGround(newPosition, out RaycastHit hit))
            {
                newPosition = Motion.GetElevation(newPosition, hit);

                _smoothingQueues.AddNewNormal(Motion.GetNormal(transform.up, hit));
                newRotation = Quaternion.FromToRotation(
                        transform.up, _smoothingQueues.AverageNormal()
                    ) * newRotation;
            }

            _rb.position = Motion.ApplyPosition(_rb.position, newPosition);
            _rb.rotation = Motion.ApplyRotation(_rb.rotation, newRotation);

            _smoothingQueues.AddNewSpeed(Motion.CurrentSpeedUnitPerSec);
            Motion.UpdateMaxSpeed();
        }

        private bool IsOnGround(Vector3 position, out RaycastHit hit)
        {
            return Physics.Raycast(
                position + (Vector3.up * _groundDetectionRange), Vector3.down,
                out hit, 2 * _groundDetectionRange, _groundLayer);
        }

        /// <summary>
        /// Repeatedly clears the movement input until the vehicle stops,
        /// then disables to component to stop calculating new movement.
        /// </summary>
        public void StopMovement()
        {
            Motion.ClearSpeeds();
            StartCoroutine(StopMovementAndDisable_Co());
        }

        private IEnumerator StopMovementAndDisable_Co()
        {
            WaitForFixedUpdate waitForFixedUpdate = new();
            float startTime = Time.time;
            float coroutineTimeoutSeconds = 5f;

            while (_smoothingQueues.AverageSpeed() != 0f)
            {
                if (Time.time - startTime > coroutineTimeoutSeconds)
                {
                    Debug.LogWarning($"Unit '{gameObject.name}' exceeded stop movement timeout");
                    break;
                }

                Motion.ClearSpeeds();
                yield return waitForFixedUpdate;
            }
            enabled = false;
        }

        private void OnEnable()
        {
            Vector3 currentNormal = _smoothingQueues.AverageNormal();
            _smoothingQueues = new SmoothingAverageQueue(
                _recentSpeedsCount, _recentNormalsCount
            );
            _smoothingQueues.Initiliaze(currentNormal, 0f);

            Motion = MotionTypeFactory.CreateMotion(
                motionTypeSelection, _ud
                );
        }

        private void OnDisable()
        {
            Motion.ClearSpeeds();
        }

        private void OnDrawGizmos()
        {
            if (Motion == null)
            {
                return;
            }

            Vector3 from = transform.position + Vector3.up * 3f;
            Color ogColor = Gizmos.color;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(from, from + transform.forward * Motion.CurrentSpeedUnitPerSec);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(from, from + transform.up * Motion.CurrentAngularSpeedDegPerSec);

            Gizmos.color = ogColor;
        }
    }
}