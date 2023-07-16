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
            if (Mathf.Abs(SteeringParams.Yaw) <= _activeLimit)
            {
                _rb.position = newPosition;
            }
            else
            {
                CurrentSpeedUnitPerSec = 0f;
            }

            _activeLimit =
                Mathf.Abs(CurrentSpeedUnitPerSec) < 0.1f ?
                _inPlaceTurnLowerLimitDegrees :
                _inPlaceTurnUpperLimitDegrees;

            _rb.rotation = newRotation;
        }

        public override float GetMaxSpeedAtTurnRate(float turnRate)
        {
            if (turnRate >= _activeLimit)
            {
                return 0f;
            }

            return base.GetMaxSpeedAtTurnRate(turnRate);
        }
    }
}