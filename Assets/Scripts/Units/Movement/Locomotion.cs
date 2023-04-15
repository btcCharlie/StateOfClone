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

        [SerializeField] private float _alignmentToleranceDegrees = 3f;

        [SerializeField] private int _recentNormalsCount = 50;

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

        private void FixedUpdate()
        {
            if (_steeringDirection == Vector3.zero)
                return;

            // Debug.Log($"Transform forward: {transform.forward.normalized}; Steering direction: {_steeringDirection.normalized}");

            // cast a ray down to get the normal of the ground
            if (!Physics.Raycast(
                _groundCheck.position, Vector3.down,
                out RaycastHit hit, 10f, _groundLayer
                ))
            {
                return;
            }

            // if (!Physics.SphereCast(
            //     _groundCheck.position, 0.5f, Vector3.down,
            //     out RaycastHit sphereHit, 10f, _groundLayer
            //     ))
            // {
            //     return;
            // }

            UpdateRecentNormals(hit.normal);

            _normal = GetNormalMovingAverage();
            _tangent = Vector3.Cross(-_normal, transform.right);
            _rigidbody.transform.rotation = Quaternion.LookRotation(_tangent, _normal);


            if (!IsAlignedTowards())
            {
                _rigidbody.transform.rotation = Quaternion.RotateTowards(
                    _rigidbody.transform.rotation,
                    Quaternion.LookRotation(_steeringDirection, transform.up),
                    _unitData.MaxTurnRateDegPerSec * Time.fixedDeltaTime
                    );
                Debug.Log("Not aligned towards target - roateted up to " +
                    $"{_unitData.MaxTurnRateDegPerSec * Time.fixedDeltaTime} degrees");
            }
            else
            {
                _rigidbody.position +=
                    _steeringForce.magnitude * Time.fixedDeltaTime * transform.forward;
                Debug.Log("Aligned towards target - moved forward by " +
                    $"{_steeringForce.magnitude * Time.fixedDeltaTime} units");
            }
        }

        private bool IsAlignedTowards()
        {
            Vector3 steeringNormalized = _steeringDirection.normalized;
            Vector3 forwardNormalized = transform.forward.normalized;
            steeringNormalized.y = forwardNormalized.y = 0f;
            float angleDifference = Vector3.Angle(steeringNormalized, forwardNormalized);
            Debug.Log($"Angle distance to align: {angleDifference}");
            return angleDifference <= _alignmentToleranceDegrees;
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
