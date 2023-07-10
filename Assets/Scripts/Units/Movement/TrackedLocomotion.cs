using UnityEngine;

namespace StateOfClone.Units
{
    public class TrackedLocomotion : Locomotion
    {
        [SerializeField] private float _inPlaceTurnAngleLimitDegrees = 30f;

        protected override void FixedUpdate()
        {
            // Calculate the new position and rotation
            CurrentSpeedUnitPerSec = Mathf.Clamp(
                SteeringParams.Speed, -_unitData.MaxSpeed, _unitData.MaxSpeed
                );
            CurrentAngularSpeedDegPerSec = Mathf.Clamp(
                SteeringParams.Yaw * _unitData.MaxTurnRate,
                 -_unitData.MaxTurnRate,
                 _unitData.MaxTurnRate
                );
            Vector3 newPosition =
                _rb.position +
                CurrentSpeedUnitPerSec * Time.fixedDeltaTime * transform.forward;

            // float yawAdjustment = CurrentSpeedUnitPerSec * _unitData.MaxTurnRate;
            // Debug.Log($"Yaw adjustment vals: {SteeringParams.Yaw} * {_unitData.MaxTurnRate}°/s = {CurrentAngularSpeedDegPerSec}°/s");
            Quaternion newRotation = Quaternion.Euler(
                0f, CurrentAngularSpeedDegPerSec * Time.fixedDeltaTime, 0f
                ) * _rb.rotation;

            // Make sure the vehicle sits flush on the ground
            if (Physics.Raycast(
                newPosition + Vector3.up * _groundDetectionRange, Vector3.down,
                out RaycastHit hit, 2 * _groundDetectionRange, _groundLayer
                ))
            {
                newPosition.y = hit.point.y;
                AddNewNormal(hit.normal);
                newRotation = Quaternion.FromToRotation(
                    transform.up, GetNormalMovingAverage()
                    ) * newRotation;
            }

            // Update the unit's position and rotation
            if (Mathf.Abs(CurrentAngularSpeedDegPerSec) < _inPlaceTurnAngleLimitDegrees)
            {
                _rb.position = newPosition;
            }
            _rb.rotation = newRotation;
        }
    }
}