using UnityEngine;

namespace StateOfClone.Units
{
    public interface IMotionType
    {
        ISpeedCalculator SpeedCalculator { get; }
        float CurrentSpeedUnitPerSec { get; }
        float CurrentAngularSpeedDegPerSec { get; }
        SteeringParams SteeringParams { get; set; }

        Vector3 ApplyPosition(Vector3 oldPosition, Vector3 newPosition);
        Quaternion ApplyRotation(Quaternion oldRotation, Quaternion newRotation);
        void UpdateMaxSpeed();
        void UpdateSpeeds();
        void ClearSpeeds();
    }
}
