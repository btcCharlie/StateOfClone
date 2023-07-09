using UnityEngine;

namespace StateOfClone.Units
{
    [RequireComponent(typeof(Unit))]
    public class WheeledLocomotion : Locomotion
    {
        protected override void FixedUpdate()
        {
            // Calculate the new position and rotation
            CurrentSpeed = Mathf.Clamp(
                SteeringParams.Speed, -_unitData.MaxSpeed, _unitData.MaxSpeed
                );
            Vector3 newPosition =
                _rb.position +
                CurrentSpeed * Time.fixedDeltaTime * transform.forward;

            float yawAdjustment =
                SteeringParams.Yaw * _unitData.MaxTurnRate * Time.fixedDeltaTime;
            // Debug.Log($"Yaw adjustment vals: {SteeringParams.Yaw} * {_unitData.MaxTurnRate}°/s * {Time.fixedDeltaTime}s = {yawAdjustment}°");
            Quaternion newRotation = Quaternion.Euler(
                0f, yawAdjustment, 0f
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
            _rb.position = newPosition;
            _rb.rotation = newRotation;
        }
    }
}