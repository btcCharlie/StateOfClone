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

        // Layer containing the ground
        public Vector3 SteeringDirection { get; set; }
        public Vector3 Velocity { get; protected set; }
        public float CurrentSpeedUnitPerSec { get; protected set; }
        public float CurrentAngularSpeedDegPerSec { get; protected set; }

        protected virtual void Awake()
        {
            _unit = GetComponent<Unit>();
            _unitData = _unit.UnitData;
            _rb = GetComponent<Rigidbody>();
            _groundLayer = LayerMask.GetMask("Ground");
            _recentNormals = new Vector3[_recentNormalsCount];
            Velocity = Vector3.zero;
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

        protected abstract void FixedUpdate();

        protected virtual void OnDisable()
        {
            Velocity = Vector3.zero;
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
        }

        public virtual void StopMovement()
        {
            SteeringDirection = Vector3.zero;
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