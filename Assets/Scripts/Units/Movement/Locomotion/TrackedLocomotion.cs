using UnityEngine;

namespace StateOfClone.Units
{
    public class TrackedLocomotion : Locomotion
    {
        [SerializeField] private float _inPlaceTurnLowerLimitDegrees = 5f;
        [SerializeField] private float _inPlaceTurnUpperLimitDegrees = 15f;
        private float _activeLimit;

        protected override void Start()
        {
            base.Start();

            _activeLimit = _inPlaceTurnLowerLimitDegrees;

        }

        protected override void ApplySteering(
            Vector3 newPosition, Quaternion newRotation
            )
        {
            Debug.Log($"Active limit: {_activeLimit}");

            if (Mathf.Abs(SteeringParams.Yaw) <= _activeLimit)
            {
                _rb.position = newPosition;
            }
            else
            {
                CurrentSpeedUnitPerSec = 0f;
            }

            if (Mathf.Abs(CurrentSpeedUnitPerSec) < 0.1f)
            {
                _activeLimit = _inPlaceTurnLowerLimitDegrees;
            }
            else
            {
                _activeLimit = _inPlaceTurnUpperLimitDegrees;
            }

            _rb.rotation = newRotation;
        }
    }
}