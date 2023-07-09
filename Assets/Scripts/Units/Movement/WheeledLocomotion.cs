using UnityEngine;
using StateOfClone.Core;

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
            Quaternion newRotation = Quaternion.Euler(
                0f,
                SteeringParams.Yaw * _unitData.MaxTurnRate * Time.fixedDeltaTime,
                0f
                ) * _rb.rotation;

            // Make sure the vehicle sits flush on the ground
            if (Physics.Raycast(
                newPosition + Vector3.up, Vector3.down,
                out RaycastHit hit, _groundDetectionRangeUnits, _groundLayer
                ))
            {
                newPosition.y = hit.point.y;
                newRotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                    ) * newRotation;
            }
            else if (Physics.Raycast(
                newPosition + Vector3.up, Vector3.down,
                out hit, -_groundDetectionRangeUnits, _groundLayer
                ))
            {
                newPosition.y = hit.point.y;
                newRotation = Quaternion.FromToRotation(
                    transform.up, hit.normal
                    ) * newRotation;
            }

            // Update the unit's position and rotation
            _rb.position = newPosition;
            _rb.rotation = newRotation;
        }
    }
}