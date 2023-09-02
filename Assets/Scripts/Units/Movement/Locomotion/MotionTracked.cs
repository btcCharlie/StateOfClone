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
        private float _inPlaceTurnLimitDegrees = 30f;

        public MotionTracked(UnitData unitData)
        {
            SpeedCalculator = new TrackedSpeedCalculator(unitData)
            {
                TurnLimit = _inPlaceTurnLimitDegrees
            };
            _actualMaxSpeed = unitData.MaxSpeed;
        }

        public Vector3 GetPosition(Vector3 oldPosition, Vector3 newPosition)
        {
            if (Mathf.Abs(SteeringParams.Yaw) > _inPlaceTurnLimitDegrees)
            {
                CurrentSpeedUnitPerSec = 0f;
            }

            return newPosition;
        }

        public Quaternion GetRotation(Quaternion oldRotation, Quaternion newRotation)
        {
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
