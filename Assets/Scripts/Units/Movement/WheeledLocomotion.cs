using UnityEngine;

namespace StateOfClone.Units
{
    public class WheeledLocomotion : Locomotion
    {
        protected override void ApplySteering(
            Vector3 newPosition, Quaternion newRotation
            )
        {
            // Update the unit's position and rotation
            _rb.position = newPosition;
            _rb.rotation = newRotation;
        }
    }
}