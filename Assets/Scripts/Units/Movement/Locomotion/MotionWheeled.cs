using UnityEngine;

namespace StateOfClone.Units
{
    public class MotionWheeled : IMotionType
    {
        public ISpeedCalculator SpeedCalculator { get; private set; }
        public float CurrentSpeedUnitPerSec { get; private set; }
        public float CurrentAngularSpeedDegPerSec { get; private set; }
        public SteeringParams SteeringParams { get; set; }

        private float _actualMaxSpeed;

        public MotionWheeled(UnitData unitData)
        {
            SpeedCalculator = new DefaultSpeedCalculator(unitData);
            _actualMaxSpeed = unitData.MaxSpeed;
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
