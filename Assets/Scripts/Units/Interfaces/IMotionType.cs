using UnityEngine;

namespace StateOfClone.Units
{
    public interface IMotionType
    {
        ISpeedCalculator SpeedCalculator { get; }
        float CurrentSpeedUnitPerSec { get; }
        float CurrentAngularSpeedDegPerSec { get; }
        SteeringParams SteeringParams { get; set; }

        Vector3 GetPosition(Vector3 oldPosition, Vector3 newPosition)
        {
            return newPosition;
        }

        Quaternion GetRotation(Quaternion oldRotation, Quaternion newRotation)
        {
            return newRotation;
        }

        void UpdateMaxSpeed();

        void UpdateSpeeds();

        Vector3 GetElevation(Vector3 position, RaycastHit hit)
        {
            position.y = hit.point.y;
            return position;
        }

        Vector3 GetNormal(Vector3 normal, RaycastHit hit)
        {
            return hit.normal;
        }

        void ClearSpeeds();
    }
}
