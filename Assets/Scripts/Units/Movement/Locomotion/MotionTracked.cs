using UnityEngine;

namespace StateOfClone.Units
{
    public class MotionTracked : IMotionType
    {
        public ISpeedCalculator SpeedCalculator { get; private set; }
        public float CurrentSpeedUnitPerSec { get; private set; }
        public float CurrentAngularSpeedDegPerSec { get; private set; }
        public SteeringParams SteeringParams { get; set; }

        private float _actualMaxSpeed;
        private float _inPlaceTurnLowerLimitDegrees = 5f;
        private float _inPlaceTurnUpperLimitDegrees = 15f;
        private float _activeLimit;

        public MotionTracked(UnitData unitData)
        {
            SpeedCalculator = new TrackedSpeedCalculator(unitData);
            _actualMaxSpeed = unitData.MaxSpeed;
            _activeLimit = _inPlaceTurnLowerLimitDegrees;
        }

        public Vector3 ApplyPosition(Vector3 oldPosition, Vector3 newPosition)
        {
            if (Mathf.Abs(SteeringParams.Yaw) > _activeLimit)
            {
                CurrentSpeedUnitPerSec = 0f;
            }

            return newPosition;
        }

        public Quaternion ApplyRotation(Quaternion oldRotation, Quaternion newRotation)
        {
            _activeLimit =
                Mathf.Abs(CurrentSpeedUnitPerSec) < 0.1f ?
                _inPlaceTurnLowerLimitDegrees :
                _inPlaceTurnUpperLimitDegrees;

            SpeedCalculator.TurnLimit = _activeLimit;

            return newRotation;
        }

        public void UpdateMaxSpeed()
        {
            _actualMaxSpeed = SpeedCalculator.GetMaxSpeedAtTurnRate(
                CurrentAngularSpeedDegPerSec
                );
        }

        public void UpdateSpeeds()
        {
            CurrentSpeedUnitPerSec = SpeedCalculator.CalculateSpeed(
                SteeringParams.Speed, _actualMaxSpeed, -_actualMaxSpeed
                );

            CurrentAngularSpeedDegPerSec = SpeedCalculator.CalculateYawTurnRate(
                SteeringParams.Yaw
                );
        }

        public void ClearSpeeds()
        {
            CurrentSpeedUnitPerSec = 0f;
            CurrentAngularSpeedDegPerSec = 0f;
            SteeringParams = SteeringParams.Zero;
        }
    }
}
